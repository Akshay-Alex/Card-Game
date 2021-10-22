using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{

    public GameObject CardPrefab;
    public GameObject CurrentCard;
    public Card CurrentCardScript;
    int CardID = 0;
    float y=0;
    Stack<GameObject> deck = new Stack<GameObject>();
    // Start is called before the first frame update

    void NewCard()
    {
        if (deck.Count < 13)
        { 
            CurrentCard = (GameObject) Instantiate(CardPrefab, new Vector3(0, y, 0), Quaternion.identity);
            y = (float) (y + 0.01);
            CurrentCard.gameObject.name = "Card" + CardID;
            CurrentCardScript = CurrentCard.GetComponent<Card>();
            CurrentCardScript.CardNumber = CardID;
            CardID++;
            deck.Push(CurrentCard);
        }
    }
    void RemoveCard()
    {
        if(deck.Count != 0)
        {
            CurrentCard = deck.Pop();
            Debug.Log(CurrentCard.GetComponent<Card>().CardNumber);
            y = (float)(y - 0.01);
            Destroy(CurrentCard);
            CardID--;
        }
       
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            NewCard();
        }
        if (Input.GetKeyDown("l"))
        {
            Debug.Log(deck.Peek());
        }
        if (Input.GetKeyDown("p"))
        {
            RemoveCard();
        }
    }
}
