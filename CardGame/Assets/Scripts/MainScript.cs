using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{

    public GameObject CardPrefab;
    public GameObject CurrentCard,CurrentCardFrontObject;
    public Card CurrentCardScript;
    public Renderer CurrentCardRenderer;
    public Material CardMaterial;
    
    int CardID = 0;
    int[] AllowedCardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    int CurrentCardNumber = 0;
    float y=0;
    Vector3 FaceDown = new Vector3(0, 0, 180);
    Vector3 FaceUp = new Vector3(0, 0, 0);
    Stack<GameObject> deck = new Stack<GameObject>();
    // Start is called before the first frame update

    void AddCard(int CardNumber)
    {
        if (deck.Count < 13)
        {
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
            CardID++;
            deck.Push(CurrentCard);
        }
    }
    public void NewCard()
    {
        CurrentCardNumber = Random.Range(1, 14);
        AddCard(CurrentCardNumber);

    }
    public void RemoveCard()
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
    public void RevealCard()
    {
        var rot = Mathf.Lerp(180, 0, 1f);
        CurrentCard.transform.rotation = Quaternion.Euler(0, 0, rot);
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
