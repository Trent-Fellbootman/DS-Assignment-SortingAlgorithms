using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : Sorter
{
    protected override void fillInSwapCommands() {
        var keys = new float[items.Length];
        for (var i = 0; i < keys.Length; i++) {
            keys[i] = items[i].key;
        }

        var commandBuffer = new List<Command>();

        for (var i = 0; i < keys.Length; i++) {
            var flag = true;
            for (var j = 0; j < items.Length - i - 1; j++) {
                commandBuffer.Add(new Command(Command.CommandType.COMPARE, j, j + 1, j));

                if (keys[j] > keys[j + 1]) {
                    flag = false;

                    commandBuffer.Add(new Command(Command.CommandType.SWAP, j, j + 1, j));
                    
                    (keys[j], keys[j + 1]) = (keys[j + 1], keys[j]);
                }
            }

            if (flag) {
                break;
            }
        }

        commands = commandBuffer.ToArray();
    }
}
