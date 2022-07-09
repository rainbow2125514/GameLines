using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameBoard : MonoBehaviour
{
    public DatasArray data;
    bool isChoose;
    GameObject objectFirst;
    int[] numberArray;
    int[] waitingBall;
    int height;
    int width;

    private void Start()
    {
        height = 5;
        width = 8;
           waitingBall = new int[3];
        numberArray = new int[40];
        for (int i = 0; i < 40; i++)
        {
            numberArray[i] = 0;
        }
        isChoose = false;
        for (int i = 0; i < 3; i++)
        {
            data = FindObjectOfType<DatasArray>();
            var count = Random.RandomRange(1, 41);
            while (numberArray[count - 1] != 0)
            {
                count = Random.RandomRange(1, 41);
            }
            var gameObject = GameObject.Find("btn" + count);
            var image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
            var color = Random.RandomRange(0, 3);
            numberArray[count - 1] = color + 1;
            image.sprite = data.ArraySprite[color];
            image.enabled = true;
        }
        data = FindObjectOfType<DatasArray>();
        CreateWaitingBall();

    }

    public void ClickButton(Button btn)
    {
        GameObject objectSecond;
        if (!isChoose)
        {
            if (HaveBall(btn))
            {
                isChoose = true;
                objectFirst = btn.gameObject;
                var outline = objectFirst.GetComponent<Outline>();
                outline.enabled = true;
            }
        }
        else
        {
            isChoose = false;
            objectSecond = btn.gameObject;
            var outlineFirst = objectFirst.GetComponent<Outline>();
            outlineFirst.enabled = false;
            var countFirst = int.Parse(objectFirst.name.Replace("btn", "")) - 1;
            var countSecond = int.Parse(objectSecond.name.Replace("btn", "")) - 1;
            if (CanMove(objectSecond, countFirst, countSecond))
            {
                var memory = MoveFromTo(objectFirst, objectSecond);
                CheckExplode();
                CheckWaitingBall(memory);
                ChangeWaitingBall();
                CheckExplode();
                CreateWaitingBall();
            }
        }
    }

    private void CheckExplode()
    {
        for(int i=0;i<height;i++)
        {
            for(int j = 0;j<width;j++)
            { 
                if(numberArray[width * i+j] != 0 && !IsWaitingBall(width * i + j))
                {
                    //check 1
                    Check1(numberArray[width * i + j],i,j);
                    Check2(numberArray[width * i + j], i, j);
                    Check3(numberArray[width * i + j], i, j);
                    Check4(numberArray[width * i + j], i, j);
                }
            }
        }

        var countExplode = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width ; j++)
            {
                if (numberArray[width * i + j] < 0)
                {
                    ExplodeBall(width * i + j);
                    countExplode += 1;
                }
            }
        }
        if(countExplode != 0)
        {
            var scoreText = GameObject.Find("Score_Text");
            var score = scoreText.GetComponent<Text>();
            score.text = (int.Parse(score.text) + countExplode).ToString();
        }
    }

    private bool IsWaitingBall(int v)
    {
        for(int i = 0;i<3;i++)
        {
            if (waitingBall[i] == v)
                return true;
        }
        return false;
    }

    private void ExplodeBall(int v)
    {
        var gameObject = GameObject.Find("btn" + (v + 1));
        var image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
        image.enabled = false;
        image.rectTransform.sizeDelta = new Vector2(34, 34);
        numberArray[v] = 0;
    }

    private void Check4(int v, int i, int j)
    {
        var count = 1;
        var limit = j > height - i -1 ? height - i - 1 : j;
        var value = Math.Abs(v);
        for (int t = 1; t <= limit; t++)
        {
            if (Math.Abs(numberArray[width * (i + t) + j - t]) == value
                && !IsWaitingBall(width * (i + t) + j - t))
                count += 1;
            else
            {
                break;
            }

        }
        if (count >= 5)
        {
            for (int t = 0; t <= limit; t++)
            {
                if (numberArray[width * (i + t) + j - t] > 0)
                    numberArray[width * (i + t) + j - t] *= -1;
            }
        }
    }

    private void Check3(int v, int i, int j)
    {
        var count = 1;
        var value = Math.Abs(v);
        for (int t = i + 1; t < height; t++)
        {
            if (Math.Abs(numberArray[width * t +j]) == value
                && !IsWaitingBall(width * t + j))
                count += 1;
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            for (int t = i; t < i + count; t++)
            {
                if (numberArray[width * t + j] > 0)
                    numberArray[width * t + j] *= -1;
            }
        }
    }

    private void Check2(int v, int i, int j)
    {
        var count = 1;
        var limit = width - j-1 > height - i-1 ? height - i - 1 : width - j - 1;
        var value = Math.Abs(v);
        for (int t = 1; t <= limit; t++)
        {
            if (Math.Abs(numberArray[width * (i+t) + j + t]) == value
                && !IsWaitingBall(width * (i + t) + j + t))
                count += 1;
            else
            {
                break;
            }
        }
        if (count >= 5)
        {
            for (int t = 0; t <= limit; t++)
            {
                if (numberArray[width * (i + t) + j + t] > 0)
                    numberArray[width * (i + t) + j + t] *= -1;
            }
        }
    }

    private void Check1(int v, int i, int j)
    {
        var count = 1;
        var value = Math.Abs(v);
        for (int t = j+1; t < width; t++)
        {
            if (Math.Abs(numberArray[width * i + t]) == value 
                && !IsWaitingBall(width * i + t))
                count += 1;
            else
            {
                break;
            }
        }
        if(count>=5)
        {
            for (int t = j ; t < j+ count; t++)
            {
                if(numberArray[width * i + t] > 0)
                    numberArray[width * i + t] *=-1;
            }
        }
    }

    private void ChangeWaitingBall()
    {
        for (int i = 0; i < 3; i++)
        {
            var gameObject = GameObject.Find("btn" + (waitingBall[i] + 1));
            var image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
            image.enabled = true;
            image.rectTransform.sizeDelta = new Vector2(34, 34);
            waitingBall[i] = -1;
        }
    }

    private void CheckWaitingBall(int memory)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var gameObject = GameObject.Find("btn" + (waitingBall[i] + 1));
            if (waitingBall[i] == -1)
            {
                var count = Random.RandomRange(1, 41);
                if (!list.Contains(count))
                    list.Add(count);
                while (numberArray[count - 1] != 0)
                {
                    if (list.Count == 40)
                    {
                        waitingBall[i] = -2;
                        break;
                    }
                    count = Random.RandomRange(1, 41);
                    if (!list.Contains(count))
                        list.Add(count);
                }
                if (waitingBall[i] == -2) continue;
                gameObject = GameObject.Find("btn" + count);
                var image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
                numberArray[count - 1] = memory;
                image.sprite = data.ArraySprite[numberArray[count - 1]-1];
                image.enabled = true;
                image.rectTransform.sizeDelta = new Vector2(12, 12);
                waitingBall[i] = count - 1;
            }
        }
    }

    private void CreateWaitingBall()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var count = Random.RandomRange(1, 41);
            if(!list.Contains(count))
                list.Add(count);
            while (numberArray[count - 1] != 0)
            {
                if(list.Count == 40)
                {
                    waitingBall[i] = -2;
                    break;
                }
                count = Random.RandomRange(1, 41);
                if (!list.Contains(count))
                    list.Add(count);
            }
            if(waitingBall[i] == -2) continue;
            waitingBall[i] = count - 1;
            var gameObject = GameObject.Find("btn" + count);
            var image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
            var color = Random.RandomRange(0, 3);
            numberArray[count - 1] = color + 1;
            image.sprite = data.ArraySprite[color];
            image.enabled = true;
            image.rectTransform.sizeDelta = new Vector2(12, 12);
        }
    }

    private bool HaveBall(Button btn)
    {
        var count = int.Parse(btn.gameObject.name.Replace("btn", "")) - 1;
        for (int i = 0; i < 3; i++)
        {
            if (waitingBall[i] == count)
                return false;
        }
        return btn.gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled;
    }

    private bool HaveBall(GameObject objectSecond)
    {
        var count = int.Parse(objectSecond.name.Replace("btn", "")) - 1;
        for (int i = 0; i < 3; i++)
        {
            if (waitingBall[i] == count)
                return false;
        }
        return objectSecond.transform.Find("Image").gameObject.GetComponent<Image>().enabled;
    }

    private int MoveFromTo(GameObject objectFirst, GameObject objectSecond)
    {
        var imageFirst = objectFirst.transform.Find("Image").gameObject.GetComponent<Image>();
        var imageSecond = objectSecond.transform.Find("Image").gameObject.GetComponent<Image>();
        imageSecond.sprite = imageFirst.sprite;
        imageFirst.enabled = false;
        imageSecond.enabled = true;
        var firstCount = int.Parse(objectFirst.name.Replace("btn", "")) - 1;
        var secondCount = int.Parse(objectSecond.name.Replace("btn", "")) - 1;
        for (int j = 0; j < 3; j++)
        {
            if (waitingBall[j] == secondCount)
            {
                waitingBall[j] = -1;
                break;
            }
        }
        imageFirst.rectTransform.sizeDelta = new Vector2(34, 34);
        imageSecond.rectTransform.sizeDelta = new Vector2(34, 34);
        int memory = numberArray[secondCount];
        numberArray[secondCount] = numberArray[firstCount];
        numberArray[firstCount] = 0;
        return memory;
    }

    private bool CanMove(GameObject objectSecond, int from, int to)
    {
        if (objectSecond.name == objectFirst.name ||
            HaveBall(objectSecond)) return false;
        int[] newList = numberArray.Clone() as int[];
        for(int i = 0;i<3;i++)
        {
            if(waitingBall[i]>0 && waitingBall[i]<width*height)
                newList[waitingBall[i]] = 0;
        }
        if (CheckMove(ref newList, from, to) == 0) return false;
        return true;
    }

    private int CheckMove(ref int[] list, int from, int to)
    {
        if (from < 0 || from >= height*width) return 0;
        if (from == to) return 1;
        int a =0, b=0, c=0, d = 0;
        if(from%width < width - 1)
        {
            if (list[from + 1] == 0)
            {
                list[from + 1] = -1;
                a = CheckMove(ref list, from + 1, to);
            }
        }
        if (from < height*width-8)
        {
            if (list[from + width] == 0)
            {
                list[from + width] = -1;
                b = CheckMove(ref list, from + width, to);
            }
        }
        if (from % width > 0)
        {
            if (list[from - 1] == 0)
            {
                list[from - 1] = -1;
                c = CheckMove(ref list, from - 1, to);
            }
        }
        if (from >= width)
        {
            if (list[from - width] == 0)
            {
                list[from - width] = -1;
                d = CheckMove(ref list, from - width, to);
            }
        }
        return a + b + c + d;
    }
}
