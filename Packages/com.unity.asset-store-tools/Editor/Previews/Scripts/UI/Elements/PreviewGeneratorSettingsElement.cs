using AssetStoreTools.Previews.Data;
using AssetStoreTools.Previews.UI.Data;
using AssetStoreTools.Validator.UI.Elements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AssetStoreTools.Previews.UI.Elements
{
    internal class PreviewGeneratorSettingsElement : VisualElement
    {
        // Data
        private IPreviewGeneratorSettings _settings;

        // UI
        private PreviewGeneratorPathsElement _previewPathsElement;
        private ToolbarMenu _generationTypeMenu;

        public PreviewGeneratorSettingsElement(IPreviewGeneratorSettings settings)
        {
            AddToClassList("preview-settings");

            _settings = settings;
            _settings.OnGenerationTypeChanged += GenerationTypeChanged;

            Create();
            Deserialize();
        }

        private void Create()
        {
            CreateGenerationType();
            CreateInputPathsElement();
        }

        private void CreateInputPathsElement()
        {
            _previewPathsElement = new PreviewGeneratorPathsElement(_settings);
            Add(_previewPathsElement);
        }

        private void CreateGenerationType()
        {
            var typeSelectionBox = new VisualElement();
            typeSelectionBox.AddToClassList("preview-settings-selection-row");

            VisualElement labelHelpRow = new VisualElement();
            labelHelpRow.AddToClassList("preview-settings-selection-label-help-row");

            Label generationTypeLabel = new Label { text = "Generation type" };
            Image categoryLabelTooltip = new Image
            {
                tooltip = "Choose the generation type for your previews.\n\n" +
                "- Native: retrieve previews from the Asset Database which are generated by Unity Editor internally\n" +
                "- High Resolution (experimental): generate previews using a custom implementation. Resulting previews are of higher resolution " +
                "than those generated by Unity Editor. Note that they may look slightly different from native previews"
            };

            labelHelpRow.Add(generationTypeLabel);
            labelHelpRow.Add(categoryLabelTooltip);

            _generationTypeMenu = new ToolbarMenu { name = "GenerationTypeMenu" };
            _generationTypeMenu.AddToClassList("preview-settings-selection-dropdown");

            typeSelectionBox.Add(labelHelpRow);
            typeSelectionBox.Add(_generationTypeMenu);

            // Append available categories
            var types = _settings.GetAvailableGenerationTypes();
            foreach (var t in types)
            {
                _generationTypeMenu.menu.AppendAction(ConvertGenerationTypeName(t), _ => _settings.SetGenerationType(t));
            }

            Add(typeSelectionBox);
        }

        private string ConvertGenerationTypeName(GenerationType type)
        {
            switch (type)
            {
                case GenerationType.Custom:
                    return "High Resolution (experimental)";
                default:
                    return type.ToString();
            }
        }

        private void GenerationTypeChanged()
        {
            var t = _settings.GetGenerationType();
            _generationTypeMenu.text = ConvertGenerationTypeName(t);
        }

        private void Deserialize()
        {
            GenerationTypeChanged();
        }
    }
}