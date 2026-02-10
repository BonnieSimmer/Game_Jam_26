using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace One_Tap_UI.UI.Others
{
    /// <summary>
    /// <para> A class that holds the data of all the colors. </para>
    /// </summary>
    [Serializable]
    public class ColorDatas
    {
        public ColorData[] colors;
        public ColorDatas(ColorData[] colors)
        {
            this.colors = colors;
        }

        public ColorData this[int index] => colors[index];
        
        public int Length => colors.Length;
        
        public void Add(ColorData color)
        {
            var newColors = new ColorData[colors.Length+1];
            for (int i = 0, count = colors.Length; i < count; i++)
            {
                newColors[i] = colors[i];
            }
            newColors[^1] = color;
            colors = newColors;
        }
    }
    
    /// <summary>
    /// <para> A class that holds the data of a color. </para>
    /// </summary>
    [Serializable]
    public class ColorData
    {
        public string name;
        public Color baseColor;
        public Color tintColor;
        public int id;
        
        public void Assign(string name, Color baseColor, Color tintColor, int id)
        {
            this.name = name;
            this.baseColor = baseColor;
            this.tintColor = tintColor;
            this.id = id;
        }
        
        public void SetTintColor(Color color) => tintColor = color;
    
        public static bool operator ==(ColorData a, string b) => a.name == b;
        public static bool operator !=(ColorData a, string b) => a.name != b;

        #region Absolutely useless, just to avoid warnings
        
        public override bool Equals(object obj)
        {
            return obj is ColorData data && name == data.name && baseColor.Equals(data.baseColor) && tintColor.Equals(data.tintColor) && id == data.id;
        }
        protected bool Equals(ColorData other)
        {
            return name == other.name && baseColor.Equals(other.baseColor) && tintColor.Equals(other.tintColor) && id == other.id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(name, baseColor, tintColor, id);
        }

        #endregion
        
        public ColorData(string name, Color baseColor, int id)
        {
            this.name = name;
            this.baseColor = baseColor;
            this.id = id;
            
            tintColor = Color.white;
        }
    }

    /// <summary>
    /// <para> A dictionary that can be used to get a color with a string. </para>
    /// </summary>
    [Serializable]
    public class StringColorDictionary
    {
        List<string> keys = new List<string>();
        List<Color> values = new List<Color>();
        
        public Color GetValue(string key)
        {
            key = key.ToLower().Replace(' ', '-');
            for (int i = 0, count = keys.Count; i < count; i++)
            {
                if (keys[i] == key) return values[i];
            }
            return Color.white;
        }
        
        public void Add(string key, Color value)
        {
            keys.Add(key);
            values.Add(value);
        }
        
        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
        
        public int Count => keys.Count;
    }
    
    /// <summary>
    /// <para> A scriptable object that can be used to change the look of the UI. </para>
    /// </summary>
    [CreateAssetMenu(fileName = "Theme Maker", menuName = "One Tap UI/UI/Theme Maker")]
    public class ThemeMaker : ScriptableObject
    {
        public Enums.TabDirection tabDirection;
        public Enums.Layout Layout;
        
        /// <summary>
        /// <para> An array of all default colors, colors that will be used the tint the default color, name of the color and the id of the color. </para>
        /// </summary>
        public ColorDatas colors = new ColorDatas(new ColorData[] { });

        /// <summary>
        /// <para> A dictionary of all the new colors, the name of the color and the color itself. </para>
        /// </summary>
        [SerializeField] private StringColorDictionary newColors = new StringColorDictionary();
        
        /// <summary>
        /// <para> Returns the color with the given name. </para>
        /// </summary>
        /// <param name="name"> The name of the color. </param>
        /// <returns> The color with the given name. </returns>
        public Color BakedColor(string name) => newColors.GetValue(name);

        /// <summary>
        /// <para> Paths of the USS file and the json file. </para>
        /// </summary>
        public class Paths
        {
            private const string ConstantUSSPath = "Assets/One Tap UI/UI/UI Toolkit/Stylesheets/";
            public string ussName;
            public string USSPath => ConstantUSSPath + ussName + ".uss";
            
            private const string ConstantJsonPath = "Assets/One Tap UI/Customize/Colors/";
            public string jsonName;
            public string JsonPath => ConstantJsonPath + jsonName + ".json";
        }
        public string SavesPath = "Assets/One Tap UI/Customize/Data/saves.json";

        /// <summary>
        /// <para> Paths of the USS file and the json file. </para>
        /// </summary>
        public Paths paths = new Paths();
        
        public List<List<ColorData>> colorDatas = new ();
        
        /// <summary>
        /// <para> Overwrites the colors in the USS file with the new colors. </para>
        /// </summary>
        public void Overwrite()
        {
            OverwriteUSS();
            OverwriteJson();
            AssetDatabase.Refresh();
        }

        private void OverwriteUSS()
        {
            newColors.Clear();
            var lines = File.ReadAllLines(paths.USSPath);
            var newLines = new string[lines.Length];
            var end = 0;
            for (int i = 0, count = lines.Length; i < count; i++)
            {
                var line = lines[i];
                if (end < colors.Length && line.Contains("--"))
                {
                    newLines[i] = WriteColorLine(line);
                    end++;
                }
                else
                {
                    newLines[i] = line;
                }
            }
            File.WriteAllLines(paths.USSPath, newLines);
        }
        
        private void OverwriteJson()
        {
            var json = JsonUtility.ToJson(new ColorDatas(colors.colors));
            File.WriteAllText(paths.JsonPath, json);
        }
        
        /// <summary>
        /// <para> Writes the appropriate color to the given line. </para>
        /// </summary>
        /// <param name="line"> The line to write. </param>
        /// <returns> The line with the color written. </returns>
        private string WriteColorLine(string line)
        {
            var name = line.Substring(line.IndexOf('-')+2, line.IndexOf(':')-6);
            for (int i = 0, j = 0, count = colors.Length; i < count; i++, j++)
            {
                if (i != 0 && colors[i].id != colors[i-1].id) j = 0;
                var color = colors[i];
                if (color == name)
                {
                    var newTintColor = color.tintColor * 255f;
                    newTintColor.a /= 255f;
                    var newColor = colors[i-j].baseColor - color.baseColor + newTintColor;
                    newColor.a = color.baseColor.a - colors[i - j].baseColor.a + newTintColor.a;
                    newColors.Add(name, newColor);
                    return line.Replace(line[(line.IndexOf(':')+1)..], $" {ColorToString(newColor)};");
                }
            }
            return "";
        }
        
        
        /// <summary>
        ///  <para> Converts a color to a string that can be used in a USS file. </para>
        /// </summary>
        /// <param name="color"> The color to convert. </param>
        /// <returns> The color as a string. </returns>
        private static string ColorToString(Color color)
        {
            return $"rgba({Mathf.RoundToInt(color.r)}, {Mathf.RoundToInt(color.g)}, {Mathf.RoundToInt(color.b)}, {color.a})";
        }
        
        /// <summary>
        ///   <para> Reads the colors from a json file and reassigns them to the colors list. </para>
        /// </summary>
        /// <param name="path"> The path of the json file. </param>
        public void ReadColors(string path)
        {
            var json = File.ReadAllText(path);
            var colorData = JsonUtility.FromJson<ColorDatas>(json);
            for (int i = 0, count = colorData.colors.Length; i < count; i++)
            {
                if (i < colors.Length)
                {
                    colors[i].Assign(colorData[i].name, colorData[i].baseColor, colorData[i].tintColor, colorData[i].id);
                    continue;
                }
                colors.Add(colorData[i]);
            }
        }
        
        public void InitializeColors()
        {
            colorDatas.Clear();
            for (int i = 0, count = colors.Length, j = -1; i < count; i++)
            {
                if (colors[i].id != j)
                {
                    colorDatas.Add(new List<ColorData>());
                    j++; // Go to the next color array
                }
                colorDatas[j].Add(colors[i]);
            }
        }

        private void OnEnable()
        {
            if (!File.Exists(SavesPath))
            {
                SavePaths();
                return;
            }
            var json = File.ReadAllText(SavesPath);
            var p = JsonUtility.FromJson<Paths>(json);
            paths.ussName = p.ussName;
            paths.jsonName = p.jsonName;
            ReadColors(paths.JsonPath);
        }

        public void SavePaths()
        {
            var js = JsonUtility.ToJson(paths);
            File.WriteAllText(SavesPath, js);
        }
    }
}