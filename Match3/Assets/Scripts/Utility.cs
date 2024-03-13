using UnityEngine;

namespace Utility
{
    public static class Utility
    {
        public static int[,] ReadCSVInt(string path)
        {
            int[,] priceList;
            TextAsset ta = Resources.Load<TextAsset>($"{path}");
            string[] lines = ta.text.Split('\n', System.StringSplitOptions.None);
            priceList = new int[lines.Length - 1, lines[0].Split(';', System.StringSplitOptions.None).Length - 1];
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(';', System.StringSplitOptions.None);

                for (int j = 1; j < values.Length; j++)
                {
                    priceList[i - 1, j - 1] = int.Parse(values[j]);
                }
            }
            return priceList;
        }

        public static string[,] ReadCSVString(string path)
        {
            string[,] priceList;
            TextAsset ta = Resources.Load<TextAsset>($"{path}");
            string[] lines = ta.text.Split('\n', System.StringSplitOptions.None);
            priceList = new string[lines.Length, lines[0].Split(';', System.StringSplitOptions.None).Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(';', System.StringSplitOptions.None);

                for (int j = 0; j < values.Length; j++)
                {
                    priceList[i, j] = values[j];
                }
            }
            return priceList;
        }
    }
}