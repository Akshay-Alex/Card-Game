using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject CurrentCard,LastCard,CurrentCardFrontObject,card;
    public Card CurrentCardScript;
    public Renderer CurrentCardRenderer;
    public Material CardMaterial;

                                                    //UI Buttons
    public Button AddButton;
    public Button RemoveButton;
    public Button RevealButton;
    public Button ShuffleButton;

    public int CardID = 0;                                                          //Stores card position
    int[] AllowedCardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };       //Possible numbers allowed on the card front face
    int CurrentCardNumber = 0,CurrentCardNewPosition = 0;                           //Stores the number and position of the Current card
    float y=0;                                                                      //Used to store y co ordinate position 
    Vector3 FaceDown = new Vector3(0, 0, 180);                                      //Stores the rotation values of a facedown card
    //Vector3 FaceUp = new Vector3(0, 0, 0);
    Stack<GameObject> deck = new Stack<GameObject>();                               // A stack of card GameObjects called deck

    #region ButtonFunctions
    public void NewCard()
    {
        CurrentCardNumber = Random.Range(1, 14);
        AddCard(CurrentCardNumber);

    }
    public void RemoveCard()
    {
        if (deck.Count != 0)
        {
            CurrentCard = deck.Pop();
            CardID--;
            Destroy(CurrentCard.gameObject);
            Debug.Log(CurrentCard);
            //Debug.Log(CurrentCard.GetComponent<Card>().CardNumber);
            y = (float)(y - 0.01);


            //GameObject DestroyCard = new GameObject();

            //CurrentCard = new GameObject();

        }

    }
    public void RevealCard()
    {
        if (CurrentCard == null)
        {
            CurrentCard = deck.Pop();
        }
        StartCoroutine(RevealCardAnimation());
        var rot = Mathf.Lerp(180, 0, 1f);
        //CurrentCard.transform.rotation = Quaternion.Euler(0, 0, rot);



    }

    public void ShuffleCards()
    {
        StartCoroutine(ShufflePosition());

    }
    #endregion
    void AddCard(int CardNumber)                    //takes card number as argument and creates a new card
    {
        if (deck.Count < 13)
        {
            CardID++;
            CurrentCard = (GameObject) Instantiate(CardPrefab, new Vector3(0, y, 0), Quaternion.Euler(FaceDown));
            y = (float) (y + 0.01);
            CurrentCard.gameObject.name = "Card" + CardID;
            CurrentCardScript = CurrentCard.GetComponent<Card>();
            CurrentCardScript.CardNumber = CardNumber;
            CurrentCardFrontObject = CurrentCard.transform.Find("front").gameObject;
            CurrentCardRenderer = CurrentCardFrontObject.GetComponent<MeshRenderer>();
            CardMaterial = Resources.Load("Materials/" + CardNumber, typeof(Material)) as Material;
            CurrentCardRenderer.material = CardMaterial;
            //mat[0] = Resources.Load("Materials/" + CardNumber, typeof(Material)) as Material;
            //CurrentCardFrontObject.GetComponent<MeshRenderer>().materials[0] = Resources.Load("Material/"+ CardNumber + ".mat", typeof(Material)) as Material;
            deck.Push(CurrentCard);
            LastCard = CurrentCard;
        }
    }
    
    IEnumerator RevealCardAnimation()                                   //stores animation procedures to reveal card
    {
        Vector3 CurrentPosition = CurrentCard.transform.position;
        Vector3 CurrentRotation = CurrentCard.transform.eulerAngles;
        LeanTween.move(CurrentCard, new Vector3(0, 1, 0), 2);
        LeanTween.rotate(CurrentCard, new Vector3(90f, 320f, 90f), 2);
        yield return new WaitForSeconds(2);
        LeanTween.move(CurrentCard, CurrentPosition, 2);
        LeanTween.rotate(CurrentCard, CurrentRotation, 2);
    }
    IEnumerator ShufflePosition()                                       //function that changes GameObject position when shuffling 
    {
        int NumberOfCards = deck.Count;
        Debug.Log(deck);
        if (NumberOfCards != 0)
        {
            deck = Shuffle(deck);                                       
            int indexValue = deck.Count;
            foreach (GameObject card in deck)               
            {
                CurrentCardNewPosition = indexValue;
                card.gameObject.name = "Card" + indexValue;
                if (indexValue % 2 == 0)
                {
                    LeanTween.moveX(card,(float)(0 -  0.5*CurrentCardNewPosition), 2);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.moveX(card, (float)(0 + 0.5 * CurrentCardNewPosition), 2);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
                CurrentCard = card;
            }
            indexValue = deck.Count;
            yield return new WaitForSeconds(1);
            foreach (GameObject card in deck)               //moveup
            {
                CurrentCardNewPosition = indexValue;
                if (indexValue % 2 == 0)
                {                   
                    LeanTween.move(card, new Vector3(-1, CurrentCardNewPosition, 0), 2);
                    yield return new WaitForSeconds((float)0.1);    
                }
                else
                {
                    LeanTween.move(card, new Vector3(1, CurrentCardNewPosition, 0), 2);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
            }
            yield return new WaitForSeconds(1);
            indexValue = deck.Count;
            foreach (GameObject card in deck)               //movex
            {
                CurrentCardNewPosition = indexValue;
                if (indexValue % 2 == 0)
                {
                    LeanTween.moveX(card, 0, 2);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.moveX(card, 0, 2);
                    yield return new WaitForSeconds((float)0.1);
                }
                Debug.Log(deck);
            }
            yield return new WaitForSeconds(1);
            foreach (GameObject card in deck)
            {
                CurrentCardNewPosition = indexValue;
                if (indexValue % 2 == 0)
                {
                    LeanTween.move(card, new Vector3(0,(float) (0.01*CurrentCardNewPosition), 0), 2);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.move(card, new Vector3(0, (float)(0.01 * CurrentCardNewPosition), 0), 2);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
            }
        }

    }
    public static Stack<GameObject> Shuffle<GameObject>(Stack<GameObject> stack)                //function that takes the deck and shuffles it randomly
    {
        return new Stack<GameObject>(stack.OrderBy(x => Random.Range(1,14)));
    }


    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("o"))
        //{
        //    NewCard();
        //}
        //
        //if (Input.GetKeyDown("s"))
        //{
        //    ShuffleCards();
        //
        //}
        //if (Input.GetKeyDown("l"))
        //{
        //    Debug.Log(deck.Peek());
        //}
        //if (Input.GetKeyDown("p"))
        //{
        //    RemoveCard();
        //}
    }
}
