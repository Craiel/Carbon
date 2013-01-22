namespace Carbon.Editor.Resource.Collada.Data
{
    using System;

    public static class ColladaDataConversion
    {
        public static float[] ConvertFloat(string rawData)
        {
            if (rawData == null)
            {
                throw new InvalidOperationException("Can not convert Float Array, Null RawData given");
            }

            string[] rawValues = rawData.Trim().Split(' ');
            float[] values = new float[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = float.Parse(rawValues[i]);
            }

            return values;
        }

        public static int[] ConvertInt(string rawData)
        {
            if (rawData == null)
            {
                throw new InvalidOperationException("Can not convert Int Array, Null RawData given");
            }

            string[] rawValues = rawData.Trim().Split(' ');
            int[] values = new int[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = int.Parse(rawValues[i]);
            }

            return values;
        }
    }
}
