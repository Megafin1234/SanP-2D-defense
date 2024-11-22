using UnityEngine;

public class UnitManager : MonoBehaviour
{
    


    public void RefreshUnitShop()
    {
        // gatchaResult = new List<ItemSO>();//초기화
        // gatchaResult.Add(childOrder[nowChildIdx].norms[0]);
        // gatchaResult.Add(childOrder[nowChildIdx].norms[1]);

        // float rateSum = 0f;
        // float tempSum=0f;
        // float randResult=0f;
        // int newItem = 0;
        // for(int i = 0; i < 17; i++)//가중치 총 합산해서 계산
        // {
        //     rateSum += probs[i]*childOrder[nowChildIdx].GoldRate[i];
        // }

        // //일단 한 번 추출
        // randResult = UnityEngine.Random.Range(0, rateSum);
        // tempSum = 0f;
        // for(int i = 0; i < 17; i++)
        // {
        //     tempSum += probs[i]*childOrder[nowChildIdx].GoldRate[i];
        //     if(randResult <= tempSum)
        //     {
        //         newItem = i;
        //         break;
        //     }
        // }


        // while(exist[newItem])
        // {
        //     //계속 추출
        //     randResult = UnityEngine.Random.Range(0, rateSum);
        //     tempSum = 0f;
        //     for(int i = 0; i < 17; i++)
        //     {
        //         tempSum += probs[i]*childOrder[nowChildIdx].GoldRate[i];
        //         if(randResult <= tempSum)
        //         {
        //             newItem = i;
        //             break;
        //         }
        //     }
        // }

        // exist[newItem]=true;
        // gatchaResult.Add(rareItems[newItem]);

        // gatchaResultPanel.SetActive(true);
        // //결과패널창에 이름이랑 이미지 보여주기.
        // for(int i = 0; i < 3; i++)
        // {
        //     resultImages[i].GetComponent<UnityEngine.UI.Image>().sprite = gatchaResult[i].image;
        //     resultNames[i].GetComponentInChildren<Text>().text = gatchaResult[i].itemName;
        // }

        // Mins_videoPanel.SetActive(true);
    }
}
