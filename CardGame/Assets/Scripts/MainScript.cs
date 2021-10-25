using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class MainScript : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject CurrentCard,CurrentCardFrontObject,card;
    public Card CurrentCardScript;
    public Renderer CurrentCardRenderer;
    public Material CardMaterial;
    private AudioSource audioSource;                                                // Used to play audio
    public AudioClip RemoveSound,NewCardSound,ShuffleSound,RevealSound;             //stores different audio clips
    public TextMeshProUGUI Warning;

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

    #region UIButtonFunctions
    public void NewCard()
    {
        if (deck.Count >= 13)
        {
            StartCoroutine(ShowWarningTextCountIsMax());
        }
        if (deck.Count < 13)
        {
            DisableUIButtons();
            CurrentCardNumber = Random.Range(1, 14);                //generating random card number
            AddCard(CurrentCardNumber);
        }

    }
    public void RemoveCard()
    {
        if (deck.Count <= 0)
        {
            StartCoroutine(ShowWarningTextCountIsZero());
        }
        
        if (deck.Count > 0)
        {
            DisableUIButtons();
            CurrentCard = deck.Pop();
            StartCoroutine(RemoveCardAnimation());
            CardID--;
            y = (float)(y - 0.01);
        }

    }
    public void RevealCard()
    {
        if (deck.Count <= 0)
        {
            StartCoroutine(ShowWarningTextCountIsZero());
        }
        if (deck.Count > 0)
        {
            CurrentCard = deck.Pop();
            deck.Push(CurrentCard);
            DisableUIButtons();
            StartCoroutine(RevealCardAnimation());
        }
    }

    public void ShuffleCards()
    {
        if(deck.Count == 0)
        {
            StartCoroutine(ShowWarningTextCountIsZero());
        }
        if (deck.Count == 1)
        {
            StartCoroutine(ShowWarningTextCountIsOne());
        }
        if (deck.Count > 1)
        { 
        DisableUIButtons();
        StartCoroutine(ShufflePosition());
        }

    }
    #endregion
    

    #region Animations
    IEnumerator AddCardAnimation()
    {
        audioSource.clip = NewCardSound;
        audioSource.Play();
        LeanTween.move(CurrentCard, new Vector3(0, y, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        EnableUIButtons();

    }
    IEnumerator RemoveCardAnimation()
    {
        audioSource.clip = RemoveSound;
        audioSource.Play();
        LeanTween.move(CurrentCard, new Vector3(100, 20, 0), 1);
        yield return new WaitForSeconds(1);
        Destroy(CurrentCard.gameObject);
        EnableUIButtons();

    }
    IEnumerator RevealCardAnimation()                                   //stores animation procedures to reveal card
    {
        Vector3 CurrentPosition = CurrentCard.transform.position;
        Vector3 CurrentRotation = CurrentCard.transform.eulerAngles;
        LeanTween.move(CurrentCard, new Vector3(0, 1, 0), 1);
        yield return new WaitForSeconds(1);
        audioSource.clip = RevealSound;
        audioSource.Play();
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
            audioSource.clip = ShuffleSound;
            audioSource.Play();                                                                                     //playing the card shuffle audio
            yield return new WaitForSeconds(1);
            EnableUIButtons();
        }

    }
    #endregion

    #region WarningText
    IEnumerator ShowWarningTextCountIsZero()
    {
        Warning.enabled = true;
        Warning.text = "No Cards left";
        yield return new WaitForSeconds(2);
        Warning.enabled = false;
    }
    IEnumerator ShowWarningTextCountIsOne()
    {
        Warning.enabled = true;
        Warning.text = "Cannot shuffle a single card";
        yield return new WaitForSeconds(2);
        Warning.enabled = false;
    }
    IEnumerator ShowWarningTextCountIsMax()
    {
        Warning.enabled = true;
        Warning.text = "Max number of cards reached";
        yield return new WaitForSeconds(2);
        Warning.enabled = false;
    }

    #endregion

    void AddCard(int CardNumber)                    //takes card number as argument and creates a new card
    {
        if (deck.Count < 13)
        {
            CardID++;
            CurrentCard = (GameObject)Instantiate(CardPrefab, new Vector3(0, y, 2), Quaternion.Euler(FaceDown));
            StartCoroutine(AddCardAnimation());
            y = (float)(y + 0.01);
            CurrentCard.gameObject.name = "Card" + CardID;
            CurrentCardScript = CurrentCard.GetComponent<Card>();
            CurrentCardScript.CardNumber = CardNumber;
            CurrentCardFrontObject = CurrentCard.transform.Find("front").gameObject;
            CurrentCardRenderer = CurrentCardFrontObject.GetComponent<MeshRenderer>();
            CardMaterial = Resources.Load("Materials/" + CardNumber, typeof(Material)) as Material;
            CurrentCardRenderer.material = CardMaterial;
            deck.Push(CurrentCard);
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
        Warning.enabled = false;
    }
    // Update is called once per frame
    
}
