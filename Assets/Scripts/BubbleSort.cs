using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : Sorter
{
    protected override void fillInSwapCommands() {
        float[] keys = new float[items.Length];
        for (int i = 0; i < keys.Length; i++) {
            keys[i] = items[i].key;
        }

        List<Command> commandBuffer = new List<Command>();

        for (int i = 0; i < keys.Length; i++) {
            for (int j = 0; j < items.Length - i - 1; j++) {
                commandBuffer.Add(new Command(Command.CommandType.COMPARE, j, j + 1, j));

                if (keys[j] > keys[j + 1]) {
                    commandBuffer.Add(new Command(Command.CommandType.SWAP, j, j + 1, j));

                    float tmp = keys[j];
                    keys[j] = keys[j + 1];
                    keys[j + 1] = tmp;
                }
            }
        }

        commands = commandBuffer.ToArray();
    }
}
