using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject CurrentCard,CardToRemove,LastCard,CurrentCardFrontObject,card;
    public Card CurrentCardScript;
    public Renderer CurrentCardRenderer;
    public Material CardMaterial;
    private AudioSource audioSource;
    public AudioClip sound;

    //UI Buttons
    public Button AddButton;
    public Button RemoveButton;
    public Button RevealButton;
    public Button ShuffleButton;

    public int CardID = 0;                                                          //Stores card position
    int CardUniqueID = 0;
    int[] AllowedCardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };       //Possible numbers allowed on the card front face
    int CurrentCardNumber = 0,CurrentCardNewPosition = 0;                           //Stores the number and position of the Current card
    float y=0;                                                                      //Used to store y co ordinate position 
    Vector3 FaceDown = new Vector3(0, 0, 180);                                      //Stores the rotation values of a facedown card
    //Vector3 FaceUp = new Vector3(0, 0, 0);
    Stack<GameObject> deck = new Stack<GameObject>();                               // A stack of card GameObjects called deck

    #region UIButtonFunctions
    public void NewCard()
    {
        if (deck.Count < 13)
        {
            DisableUIButtons();
            CurrentCardNumber = Random.Range(1, 14);
            AddCard(CurrentCardNumber);
        }

    }
    public void RemoveCard()
    {
        if(deck.Count > 0)
        { 
            CurrentCard = deck.Pop();
            Destroy(CurrentCard.gameObject);
            CardID--;
            y = (float)(y - 0.01);
        }

    }
    public void RevealCard()
    {
        CurrentCard = deck.Pop();
        deck.Push(CurrentCard);
        DisableUIButtons();
        StartCoroutine(RevealCardAnimation());
        //EnableUIButtons();
        //var rot = Mathf.Lerp(180, 0, 1f);
        //CurrentCard.transform.rotation = Quaternion.Euler(0, 0, rot);



    }

    public void ShuffleCards()
    {
        DisableUIButtons();
        StartCoroutine(ShufflePosition());
        

    }
    #endregion
    void AddCard(int CardNumber)                    //takes card number as argument and creates a new card
    {
        if (deck.Count < 13)
        {
            CardID++;
            CardUniqueID++;
            CurrentCard = (GameObject) Instantiate(CardPrefab, new Vector3(0, y, 2), Quaternion.Euler(FaceDown));
            StartCoroutine(AddCardAnimation());
            y = (float) (y + 0.01);
            CurrentCard.gameObject.name = "Card" + CardID;
            CurrentCardScript = CurrentCard.GetComponent<Card>();
            CurrentCardScript.CardNumber = CardNumber;
            CurrentCardScript.CardUniqueID = CardUniqueID;
            CurrentCardFrontObject = CurrentCard.transform.Find("front").gameObject;
            CurrentCardRenderer = CurrentCardFrontObject.GetComponent<MeshRenderer>();
            CardMaterial = Resources.Load("Materials/" + CardNumber, typeof(Material)) as Material;
            CurrentCardRenderer.material = CardMaterial;
            //mat[0] = Resources.Load("Materials/" + CardNumber, typeof(Material)) as Material;
            //CurrentCardFrontObject.GetComponent<MeshRenderer>().materials[0] = Resources.Load("Material/"+ CardNumber + ".mat", typeof(Material)) as Material;
            deck.Push(CurrentCard);
            //LastCard = CurrentCard;
        }
    }
    IEnumerator AddCardAnimation()
    {
        LeanTween.move(CurrentCard, new Vector3(0, y, 0), 1);
        yield return new WaitForSeconds(1);
        EnableUIButtons();
    }
    
    IEnumerator RevealCardAnimation()                                   //stores animation procedures to reveal card
    {
        Vector3 CurrentPosition = CurrentCard.transform.position;
        Vector3 CurrentRotation = CurrentCard.transform.eulerAngles;
        LeanTween.move(CurrentCard, new Vector3(0, 1, 0), 1);
        yield return new WaitForSeconds(1);
        LeanTween.rotate(CurrentCard, new Vector3(90f, 320f, 90f), 2);
        yield return new WaitForSeconds(2);
        LeanTween.rotate(CurrentCard, CurrentRotation, 1);
        yield return new WaitForSeconds(1);
        LeanTween.move(CurrentCard, CurrentPosition, 1);
        yield return new WaitForSeconds(1);
        EnableUIButtons();
    }
    IEnumerator ShufflePosition()                                       //function that changes order of cards in both 3d space and deck
    {
        int NumberOfCards = deck.Count;
        Debug.Log(deck);
        if (NumberOfCards != 0)
        {
            deck = Shuffle(deck);                                       //function to shuffle the deck gameObjects                           
            int indexValue = deck.Count;
            int TotalCards = deck.Count;

            foreach (GameObject card in deck)                           //start of shuffle animation
            {
                if(indexValue == TotalCards)
                {
                    CurrentCard = card;
                }
                CurrentCardNewPosition = indexValue - 1;
                card.gameObject.name = "Card" + indexValue;
                if (indexValue % 2 == 0)                                                    //moving half of the cards left, and the other half to the right
                {
                    LeanTween.moveX(card,(float)(0 -  0.7*CurrentCardNewPosition), 1);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.moveX(card, (float)(0 + 0.7 * CurrentCardNewPosition), 1);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
            }
            indexValue = deck.Count;
            yield return new WaitForSeconds(1);
            foreach (GameObject card in deck)                                                   //moving the cards up  
            {
                CurrentCardNewPosition = indexValue - 1;
                if (indexValue % 2 == 0)
                {                   
                    LeanTween.move(card, new Vector3(-1, CurrentCardNewPosition, 0), 1);
                    yield return new WaitForSeconds((float)0.1);    
                }
                else
                {
                    LeanTween.move(card, new Vector3(1, CurrentCardNewPosition, 0), 1);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
            }
            yield return new WaitForSeconds(1);
            indexValue = deck.Count;
            foreach (GameObject card in deck)                                                   //merging the cards on the left with the cards on the right
            {
                CurrentCardNewPosition = indexValue - 1;
                if (indexValue % 2 == 0)
                {
                    LeanTween.moveX(card, 0, 1);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.moveX(card, 0, 1);
                    yield return new WaitForSeconds((float)0.1);
                }
                Debug.Log(deck);
            }
            yield return new WaitForSeconds(1);
            foreach (GameObject card in deck)                                                   //returning the cards to the table
            {
                CurrentCardNewPosition = indexValue - 1;
                if (indexValue % 2 == 0)
                {
                    LeanTween.move(card, new Vector3(0,(float) (0.01*CurrentCardNewPosition), 0), 1);
                    yield return new WaitForSeconds((float)0.1);
                }
                else
                {
                    LeanTween.move(card, new Vector3(0, (float)(0.01 * CurrentCardNewPosition), 0), 1);
                    yield return new WaitForSeconds((float)0.1);
                }

                indexValue--;
            }
            audioSource.Play();                                                                                     //playing the card shuffle audio
            yield return new WaitForSeconds(1);
            EnableUIButtons();
        }

    }
    public static Stack<GameObject> Shuffle<GameObject>(Stack<GameObject> stack)                //function that takes the deck and shuffles it randomly
    {
        return new Stack<GameObject>(stack.OrderBy(x => Random.Range(1,14)));
    }
    void DisableUIButtons()                                                     //used to disable UI buttons so that user does not interrupt animation
    {
        AddButton.enabled = false;
        RemoveButton.enabled = false;
        RevealButton.enabled = false;
        ShuffleButton.enabled = false;
    }
    void EnableUIButtons()                                                       //used to enable UI buttons 
    {
        AddButton.enabled = true;
        RemoveButton.enabled = true;
        RevealButton.enabled = true;
        ShuffleButton.enabled = true;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    
}
