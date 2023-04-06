namespace GeneticAlgorithm.Extensions
{
    public static class SequenceExtension
    {
        public static byte[] ToSequence(this byte[] array, int startNumber = 0)
        {
            byte[] copy = (byte[])array.Clone();
            for (int i = 0; i < copy.Length; i++)
            {
                for (int j = i + 1; j < copy.Length; j++)
                {
                    if (copy[j].CompareTo(copy[i]) < 0)
                    {
                        byte tmp = copy[j];
                        copy[j] = copy[i];
                        copy[i] = tmp;
                    }
                }
            }
            bool[] used = new bool[copy.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                for (int j = 0; j < copy.Length; j++)
                {
                    if (array[i] == copy[j] && !used[j])
                    {
                        array[i] = checked((byte)(j + startNumber));
                        used[j] = true;
                        break;
                    }
                }
            }
            return array;
        }

        public static int[] ToSequence(this int[] array, int startNumber = 0)
        {
            int[] copy = (int[])array.Clone();
            for (int i = 0; i < copy.Length; i++)
            {
                for (int j = i + 1; j < copy.Length; j++)
                {
                    if (copy[j].CompareTo(copy[i]) < 0)
                    {
                        int tmp = copy[j];
                        copy[j] = copy[i];
                        copy[i] = tmp;
                    }
                }
            }
            bool[] used = new bool[copy.Length];
            for (int i = 0; i < copy.Length; i++)
            {
                for (int j = 0; j < copy.Length; j++)
                {
                    if (array[i] == copy[j] && !used[j])
                    {
                        array[i] = j + startNumber;
                        used[j] = true;
                        break;
                    }
                }
            }
            return array;
        }
    }
}
