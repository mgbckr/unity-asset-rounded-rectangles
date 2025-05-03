// INPUT
// float2 uv;
// float2 p1;
// float2 p2;

// OUTPUT
// float s;

float g = 
	(uv.x - p1.x) * (p2.y - p1.y) 
	- (uv.y - p1.y) * (p2.x - p1.x);
s = saturate(sign(g));