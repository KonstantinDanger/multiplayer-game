using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageProcessorWindow : EditorWindow
{
    private const string WindowName = "Image Processor";

    [SerializeField] private List<Texture2D> _images = new();
    [SerializeField] private ImageProcessor _imageProcessor = new();

    [MenuItem("Tools/ImageProcessor")]
    public static void Open()
    {
        var window = GetWindow<ImageProcessorWindow>();
        window.titleContent = new UnityEngine.GUIContent(WindowName);
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        SerializedObject serObject = new SerializedObject(this);

        // List creation
        SerializedProperty listProperty = serObject.FindProperty(nameof(_images));

        ListView listView = new ListView();
        listView.BindProperty(listProperty);

        listView.showAddRemoveFooter = true;
        listView.reorderable = true;
        listView.showBorder = true;

        listView.makeItem = () => new PropertyField();

        listView.bindItem = (element, index) =>
        {
            SerializedProperty prop = listProperty.GetArrayElementAtIndex(index);
            (element as PropertyField).BindProperty(prop);
            element.SetEnabled(true);
        };

        root.Add(listView);
        //

        var outputDirectoryField = new TextField("Output directory...");
        outputDirectoryField.value = "_Textures/Processed";
        root.Add(outputDirectoryField);

        // Settings creation
        root.Add(new Label("Settings"));
        SerializedProperty processorProperty = serObject.FindProperty(nameof(_imageProcessor));
        PropertyField processorField = new PropertyField(processorProperty);
        processorField.BindProperty(serObject);
        root.Add(processorField);
        //

        void clickEvent()
        {
            IEnumerable<Texture2D> processed = _imageProcessor.ProcessImages(_images);

            foreach (Texture2D texture in processed)
            {
                CreateTextureAssetFrom(texture, outputDirectoryField.value);
            }
        }

        var button = new Button(clickEvent)
        {
            name = "Process button",
            text = "Process",
        };

        root.Add(button);
    }

    private void CreateTextureAssetFrom(Texture2D texture, string outputDir)
    {
        if (texture == null)
            throw new ArgumentException("No texture found");

        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, outputDir, $"{texture.name}-stylized.png");

        //UnityEngine.Debug.Log("texture name:  " + texture.name);
        //UnityEngine.Debug.Log("Path is  " + path);
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
    }

    [Serializable]
    private class ImageProcessor
    {
        [Header("Oil painting effect")]
        [SerializeField, Range(0, 20)] private int _radius;
        [SerializeField, Range(0f, 100f)] private float _intensityLevels;

        [SerializeField] private bool _oilPaintify = true;

        [Header("Pixelate")]
        [SerializeField, Range(1, 100)] private int _cellSize;

        [SerializeField] private bool _pixelate = true;

        public IEnumerable<Texture2D> ProcessImages(IEnumerable<Texture2D> textures)
        {
            List<Texture2D> images = new();

            foreach (var item in textures)
            {
                Texture2D result = new Texture2D(item.width, item.height);
                result.SetPixels(item.GetPixels());
                result.Apply();

                result = OilPaintify(result);
                result = Pixelate(result);

                result.name = item.name;
                result.Apply();
                images.Add(result);
            }

            return images;
        }

        private Texture2D OilPaintify(Texture2D source)
        {
            if (!_oilPaintify)
                return source;

            int width = source.width;
            int height = source.height;
            Texture2D result = new Texture2D(width, height);
            Color[] sourcePixels = source.GetPixels();
            Color[] resultPixels = new Color[sourcePixels.Length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int[] intensityCount = new int[(int)_intensityLevels + 1];
                    float[] sumR = new float[(int)_intensityLevels + 1];
                    float[] sumG = new float[(int)_intensityLevels + 1];
                    float[] sumB = new float[(int)_intensityLevels + 1];

                    for (int ny = -_radius; ny <= _radius; ny++)
                    {
                        for (int nx = -_radius; nx <= _radius; nx++)
                        {
                            int ix = Mathf.Clamp(x + nx, 0, width - 1);
                            int iy = Mathf.Clamp(y + ny, 0, height - 1);
                            Color col = sourcePixels[iy * width + ix];

                            float gray = (col.r + col.g + col.b) / 3.0f;
                            int intensity = (int)((gray * _intensityLevels));

                            intensityCount[intensity]++;
                            sumR[intensity] += col.r;
                            sumG[intensity] += col.g;
                            sumB[intensity] += col.b;
                        }
                    }

                    int maxIndex = 0;
                    int maxCount = 0;
                    for (int i = 0; i < intensityCount.Length; i++)
                    {
                        if (intensityCount[i] > maxCount)
                        {
                            maxCount = intensityCount[i];
                            maxIndex = i;
                        }
                    }

                    resultPixels[y * width + x] = new Color(
                        sumR[maxIndex] / maxCount,
                        sumG[maxIndex] / maxCount,
                        sumB[maxIndex] / maxCount
                    );
                }
            }

            result.SetPixels(resultPixels);
            result.Apply();
            return result;
        }
        private Texture2D Pixelate(Texture2D source)
        {
            if (!_pixelate)
                return source;

            if (source == null)
                throw new System.ArgumentNullException("source");

            if (_cellSize < 1)
                _cellSize = 1;

            int w = source.width, h = source.height;
            Color[] srcPixels = source.GetPixels();
            Color[] dst = new Color[w * h];

            for (int by = 0; by < h; by += _cellSize)
            {
                for (int bx = 0; bx < w; bx += _cellSize)
                {
                    int cellW = Mathf.Min(_cellSize, w - bx);
                    int cellH = Mathf.Min(_cellSize, h - by);

                    Color avg = Color.clear;
                    int count = cellW * cellH;

                    for (int dy = 0; dy < cellH; dy++)
                        for (int dx = 0; dx < cellW; dx++)
                            avg += srcPixels[(by + dy) * w + (bx + dx)];

                    avg.r /= count; avg.g /= count;
                    avg.b /= count; avg.a /= count;

                    for (int dy = 0; dy < cellH; dy++)
                        for (int dx = 0; dx < cellW; dx++)
                            dst[(by + dy) * w + (bx + dx)] = avg;
                }
            }

            var result = new Texture2D(w, h, source.format, false);
            result.SetPixels(dst);
            result.Apply();
            return result;
        }
    }
}

