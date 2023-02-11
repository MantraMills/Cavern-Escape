using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulBlocks : MonoBehaviour
{
    public Spirit refSpiritScript;
    public Player refPlayerScript;
    public MovingPlat plat9Script;

    public GameObject[] soulBlocks;
    public GameObject effectSpawn;
    public GameObject effect;

    public bool blockMode;

    // Start is called before the first frame update
    void Start()
    {
        print(soulBlocks.Length);
        ToggleAllBlocks(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(refPlayerScript.spiritMode == true)
        {
            if (!blockMode)
            {
                blockMode = true;
                ToggleAllBlocks(true);

            }
            else DeadBlocks();
        }
        else if(refPlayerScript.spiritMode == false)
        {
            blockMode = false;
            ToggleAllBlocks(false);
        }
    }

    public void DeadBlocks()
    {
        int listCount = soulBlocks.Length;
        int currentCount = 0;
        GameObject currentBlock;
        for(int i=0; i < listCount; i++)
        {
            currentBlock = soulBlocks[i];
            if (!currentBlock.activeSelf)
            {
                currentCount++;
            }
        }
        if(currentCount == listCount && name != "SpiritBlocks 3")
        {
            Instantiate(effect, effectSpawn.transform.position, Quaternion.identity);
            Destroy(gameObject);

            print("not the third wall");
        }
        if (currentCount == listCount && name == "SpiritBlocks 3")
        {
            Instantiate(effect, effectSpawn.transform.position, Quaternion.identity);
            Destroy(gameObject);

            plat9Script.plat9 = true;

            print("yup im the third wall");
        }
    }

    void ToggleAllBlocks(bool activeState)
    {
        print("HI WEàRE IN HERE");
        int listCount = soulBlocks.Length;
        GameObject currentBlock = null;

        for (int i = 0; i < listCount; i++)
        {
            currentBlock = soulBlocks[i];
            print("i" + i);
            currentBlock.SetActive(activeState);
        }
    }
}
