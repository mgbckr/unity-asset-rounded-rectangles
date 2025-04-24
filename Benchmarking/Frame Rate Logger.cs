using UnityEngine;
using System.Collections.Generic;

public class FrameRateLogger : MonoBehaviour
{

    // settings

    public float burnInStart = 3f;
    public float burnInBenchmark = 3f;

    public int measuringRounds = 3;
    public float measuringRoundTime = 15f;

    public string[] benchmarkNames = new string[] {
        "Rounded Corner Step",
        "Rounded Corner Step Composite"};
    private string objectName = "Quad"; // or assign in inspector

    public float rotationSpeed = 360f;

    // internal state

    private bool done = false;
    private bool triggerBenchmarkSwitch = false;
    private bool isBurning = false;
    private bool isBurningIn = true;
    private float timeToBurn = -1f;
    private string benchmarkName = null;
    private int nextBenchmarkIndex = 0;
    private int currentRound = 0;

    private Dictionary<string, float[]> stats = new Dictionary<string, float[]>();

    private float elapsedTime = 0f;
    private int frameCount = 0;

    private Transform benchmarkTransform;

    void Start()
    {
        if (burnInStart > 0)
        {
            Debug.Log($"[FrameRateLogger] Starting burn in: {burnInStart} seconds");
            timeToBurn = burnInStart;
            isBurning = true;
            isBurningIn = true;
        }
        else
        {
            Debug.Log($"[FrameRateLogger] Starting immediately.");
            isBurning = false;
            isBurningIn = false;
        }
    }

    bool InitBenchmark(string benchmarkName) 
    {
        Debug.Log($"[FrameRateLogger] Init benchmark: {benchmarkNames[nextBenchmarkIndex]}");
        
        // Find the child by name
        benchmarkTransform = transform.Find(benchmarkName);
        benchmarkTransform.gameObject.SetActive(true);


        if (benchmarkTransform == null)
        {
            Debug.LogError("Benchmark not found: '" + objectName);
            return false;
        }

        // init stats
        stats.Add(benchmarkName, new float[measuringRounds]);

        // reset rounds
        currentRound = 0;

        return true;
    }


    void DeactivateAllBenchmarks()
    {
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void LogFrameRate()
    {

        if (done)
        {
            return;
        }

        // update stats
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        // wait for burn time to finish
        if (timeToBurn > 0)
        {
            timeToBurn -= Time.unscaledDeltaTime;
            return;
        }
        else if (isBurning)
        {
            isBurning = false;

            if (isBurningIn)
            {
                Debug.Log($"[FrameRateLogger] Finished burn in: {burnInStart} seconds");
                triggerBenchmarkSwitch = true;
                isBurningIn = false;
            }
            else
            {
                Debug.Log($"[FrameRateLogger] Finished burning: {burnInBenchmark} seconds");
                elapsedTime = 0f;
                frameCount = 0;
            }

        }

        if (triggerBenchmarkSwitch)
        {

            triggerBenchmarkSwitch = false;

            if (nextBenchmarkIndex > 0)
            {

                Debug.Log($"[FrameRateLogger] Finished logging for {benchmarkName}");

                // mean FPS
                float mean = 0f;
                for (int i = 0; i < measuringRounds; i++)
                {
                    mean += stats[benchmarkName][i];
                }
                mean /= measuringRounds;
                Debug.Log($"[FrameRateLogger] Mean FPS: {mean:F2}");
            }

            // select benchmark
            if (nextBenchmarkIndex < benchmarkNames.Length)
            {

                DeactivateAllBenchmarks();
                benchmarkName = benchmarkNames[nextBenchmarkIndex];
                InitBenchmark(benchmarkName);
                nextBenchmarkIndex++;

                if (burnInBenchmark > 0)
                {
                    Debug.Log($"[FrameRateLogger] Waiting for {burnInBenchmark} seconds before starting next benchmark.");
                    timeToBurn = burnInBenchmark;
                    isBurning = true;
                }
                else
                {
                    Debug.Log($"[FrameRateLogger] Starting next benchmark immediately.");
                    isBurning = false;
                }

                elapsedTime = 0f;
                frameCount = 0;
                return;
            }
            else 
            {                
                DeactivateAllBenchmarks();
                benchmarkName = null;
                done = true;
                Debug.Log($"[FrameRateLogger] No benchmarks left.");

                // save stats to file
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, "benchmark_results.csv");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine("Benchmark,Round,Interval,Mean FPS");
                    foreach (var benchmark in stats)
                    {
                        for (int i = 0; i < measuringRounds; i++)
                        {
                            writer.WriteLine($"{benchmark.Key},{i + 1},{measuringRoundTime},{benchmark.Value[i]}");
                        }
                    }
                }
            }

        }

        // logging
        if (elapsedTime >= measuringRoundTime)
        {
            float averageFPS = frameCount / elapsedTime;
            float averageFrameTime = 1000f / averageFPS;

            Debug.Log($"[FrameRateLogger] Average over {measuringRoundTime} seconds: {averageFrameTime:F2} ms/frame ({averageFPS:F2} FPS)");

            elapsedTime = 0f;
            frameCount = 0;
            if (benchmarkName != null)
            {
                stats[benchmarkName][currentRound] = averageFPS;
            }
            currentRound++;

            if (currentRound >= measuringRounds)
            {
                triggerBenchmarkSwitch = true;
            }
        }
    }

    void Update()
    {
        LogFrameRate();
    }
}
