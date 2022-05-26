using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort : Sorter
{
    protected override void fillInSwapCommands()
    {
        var keys = new float[items.Length];

        for (var i = 0; i < keys.Length; i++)
        {
            keys[i] = items[i].key;
        }

        var commandBuffer = new List<Command>();

        quickSort(keys, 0, keys.Length - 1, commandBuffer);

        // bool flag = true;

        // for (int i = 0; i < keys.Length - 1; i++)
        // {
        //     if (keys[i] > keys[i + 1]) { flag = false; }
        // }

        // Debug.Log(flag);

        commands = commandBuffer.ToArray();
    }

    private void quickSort(float[] array, int begin, int end, List<Command> buffer)
    {
        if (end - begin + 1 <= 1)
        {
            return;
        } else
        {
            var median = (begin + end) / 2;
            
            buffer.Add(new Command(Command.CommandType.COMPARE, begin, median, median, median));
            if (array[begin] > array[median])
            {
                buffer.Add(new Command(Command.CommandType.SWAP, begin, median, median, median));
                (array[begin], array[median]) = (array[median], array[begin]);
            }

            buffer.Add(new Command(Command.CommandType.COMPARE, begin, end, median, median));
            if (array[begin] > array[end])
            {
                buffer.Add(new Command(Command.CommandType.SWAP, begin, end, median, median));
                (array[begin], array[end]) = (array[end], array[begin]);
            }

            buffer.Add(new Command(Command.CommandType.COMPARE, median, end, median, median));
            if (array[median] > array[end])
            {
                buffer.Add(new Command(Command.CommandType.SWAP, median, end, median, median));
                (array[median], array[end]) = (array[end], array[median]);
            }

            if (end - begin + 1 <= 3)
            {
                return;
            }

            buffer.Add(new Command(Command.CommandType.SWAP, median, end - 1, median, median));
            (array[median], array[end - 1]) = (array[end - 1], array[median]);

            int i = begin + 0, j = end - 1;
            for (;;)
            {
                while (array[++i] < array[end - 1]) {
                    buffer.Add(new Command(Command.CommandType.COMPARE, i, end - 1, median, end - 1));
                }
                buffer.Add(new Command(Command.CommandType.COMPARE, i, end - 1, median, end - 1));

                while (array[--j] > array[end - 1]) {
                    buffer.Add(new Command(Command.CommandType.COMPARE, j, end - 1, median, end - 1));
                }
                buffer.Add(new Command(Command.CommandType.COMPARE, j, end - 1, median, end - 1));

                if (i < j)
                {
                    buffer.Add(new Command(Command.CommandType.SWAP, i, j, median, end - 1));
                    (array[i], array[j]) = (array[j], array[i]);
                } else
                {
                    break;
                }
            }

            buffer.Add(new Command(Command.CommandType.SWAP, i, end - 1, median, end - 1));
            (array[i], array[end - 1]) = (array[end - 1], array[i]);

            quickSort(array, begin, i - 1, buffer);
            quickSort(array, i + 1, end, buffer);
        }
    }
}