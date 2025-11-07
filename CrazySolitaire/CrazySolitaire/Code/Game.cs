using CrazySolitaire.Properties;
using System;
using System.Media;
using Timer = System.Windows.Forms.Timer;

namespace CrazySolitaire;

public enum CardType
{
    ACE,
    _2,
    _3,
    _4,
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    JACK,
    QUEEN,
    KING
}

public enum Suit
{
    DIAMONDS,
    SPADES,
    HEARTS,
    CLUBS
}

public interface IDragFrom
{
    public void RemCard(Card card);
    public void AddCard(Card card);
}

public interface IFindMoveableCards
{
    public List<Card> FindMoveableCards();
}

public interface IDropTarget
{
    public void DragOver(Card c);
    public bool CanDrop(Card c);
    public void DragEnded();
    public void Dropped(Card c);
}

public static class MyExtensions
{
    public static void AddCard(this Control control, Card card)
    {
        if (card is not null)
        {
            control.Controls.Add(card.PicBox);
        }
    }
    public static void RemCard(this Control control, Card card)
    {
        if (card is not null)
        {
            control.Controls.Remove(card.PicBox);
        }
    }
}

public class Deck
{
    internal Queue<Card> cards;

    public Deck()
    {
        RegeneratePool();
    }

    private void RegeneratePool()
    {
        cards = new();
        foreach (var cardType in Enum.GetValues<CardType>())
        {
            foreach (var suit in Enum.GetValues<Suit>())
            {
                cards.Enqueue(new(cardType, suit));
            }
        }
        // shuffle
        Random rng = new();
        cards = new(cards.OrderBy(_ => rng.Next()));
    }

    public bool IsEmpty() => cards.Count == 0;
    public Card Acquire() => (cards.Count > 0 ? cards.Dequeue() : null);
    public void Release(Card c) => cards.Enqueue(c);
}

public class Card
{
    public CardType Type { get; set; }
    public Suit Suit { get; set; }
    public bool FaceUp { get; private set; }
    public bool IsWildCard { get; set; } = false; //flag to refer to when checking if a card is a wildcard in methods
    public PictureBox PicBox { get; private set; }
    internal TableauStack IsIn { get; set; }//tableau stack that card is currently in
    

    /// <summary>
    /// Gets the appropriate image for the card based on its current state
    /// Returns the card back image if the card is face down, a special image if it's a wild card,
    /// or the standard card face image corresponding to its type and suit when face up.
    /// </summary>
    public Bitmap PicImg
    {
        get
        {
            // Show the card back when it's face down
            if (!FaceUp)
                return Resources.back_green;

            // Show the special wild card image when applicable
            if (IsWildCard)
                return Resources.wild_card; 

            // Retrieve the appropriate face image from resources
            return Resources.ResourceManager.GetObject($"{Type.ToString().Replace("_", "").ToLower()}_of_{Suit.ToString().ToLower()}") as Bitmap;
        }
    } 

    private Point dragOffset;
    private Point relLocBeforeDrag;
    private Control conBeforeDrag;
    private IDropTarget lastDropTarget;

    public Card(CardType type, Suit suit)
    {
        Type = type;
        Suit = suit;
        FaceUp = true;
        SetupPicBox();
    }

    /// <summary>
    /// Creates the picture box of the card and handles the picking up, dragging, and placing of the card.
    /// </summary>
    private void SetupPicBox()
    {
        PicBox = new()
        {
            Width = 90,
            Height = 126,
            BackgroundImageLayout = ImageLayout.Stretch,
            BorderStyle = BorderStyle.FixedSingle,
            BackgroundImage = PicImg
        };
        PicBox.Click += (sender, e) => {
            if (!FaceUp && Game.CanFlipOver(this))
            {
                FlipOver();
                ScoreManager.AddPoints(5);      // Reward player 5 points when the flip over a card

                var player = new SoundPlayer(Resources.flip);   
                player.Play();
            }
        };

        // handles picking up cards, changed to handle dragging multiple valid cards
        PicBox.MouseDown += (sender, e) => {
             if (e.Button == MouseButtons.Left && Game.IsCardMovable(this))
            {
                
                //if in a tableau stack
                if (this.IsIn != null)
                {
                    
                    List<Card> movables = this.IsIn.FindMoveableCards(); //grab a list of all the cards that are draggable
                    
                    int index = movables.IndexOf(this);//find the point in the stack that is being dragged

                    //drag all the other cards
                    while (index < movables.Count)
                    {
                        //handle the card being dragged by the cursor
                        if(index == 0)
                        {
                            //in the following. movables[index] refers to a specific card in the list of movable cards
                            FrmGame.DragCard(movables[index]); 
                            movables[index].dragOffset = e.Location; //set drag offset equal to the location of the cursor on the picBox
                            movables[index].conBeforeDrag = movables[index].PicBox.Parent; //grab the control that the card is in
                            movables[index].relLocBeforeDrag = movables[index].PicBox.Location; //grab the coordinates of the card relative to its parent
                            conBeforeDrag.RemCard(movables[index]);
                            FrmGame.Instance.AddCard(movables[index]);
                            Point loc = movables[index].conBeforeDrag.Location; //create a point for the cards location after being picked up
                            movables[index].PicBox.Location = new Point(loc.X, loc.Y + (relLocBeforeDrag.Y)); //set card's new location based on screen coordinates
                            movables[index].PicBox.BringToFront();
                            index++;
                        }
                        //handle the rest of the movables
                        else
                        {
                            //same as index 0 except the new point loc
                            FrmGame.DragCard(movables[index]);
                            movables[index].dragOffset = e.Location;
                            movables[index].conBeforeDrag = movables[index].PicBox.Parent;
                            movables[index].relLocBeforeDrag = movables[index].PicBox.Location;
                            conBeforeDrag.RemCard(movables[index]);
                            FrmGame.Instance.AddCard(movables[index]);
                            Point loc = movables[index].conBeforeDrag.Location;
                            movables[index].PicBox.Location = new Point(1000,1000); //set the location of each card below the main card being dragged to off-screen
                            movables[index].PicBox.BringToFront();
                            index++;
                        }
                        
                    }
                }
                //otherwise card is coming from talon or foundation
                else
                {
                    FrmGame.DragCard(this);
                    dragOffset = e.Location;
                    conBeforeDrag = PicBox.Parent;
                    relLocBeforeDrag = PicBox.Location;

                    conBeforeDrag.RemCard(this);
                    FrmGame.Instance.AddCard(this);
                    PicBox.Location = e.Location;
                    PicBox.BringToFront();
                }
            }
        };
        //handles dropping cards, altered to handle droping a linkedlist of cards
        PicBox.MouseUp += (sender, e) => {
            if (FrmGame.CurDragCards.Contains(this))
            {
                //cards deemed valid to be placed
                if (lastDropTarget is not null && lastDropTarget.CanDrop(this))
                {
                    //iterate through each card that is currently being dragged and drop them on the control
                    foreach (Card c in FrmGame.CurDragCards)
                    {
                        FrmGame.CardsDraggedFrom.RemCard(c);
                        lastDropTarget.Dropped(c);
                        c.PicBox.BringToFront();
                    }

                }
                //cards deemed invalid
                else
                {
                    //otherwise put the cards back where they belong
                    foreach (Card c in FrmGame.CurDragCards)
                    {
                        FrmGame.Instance.RemCard(c);
                        conBeforeDrag?.AddCard(c);
                        c.PicBox.Location = c.relLocBeforeDrag;
                        c.PicBox.BringToFront();
                    }
                }
                FrmGame.StopDragCard(this); 
                Game.CallDragEndedOnAll();//remove any remaining highlighted columns
            }
        };

        //handles dragging cards, slightly altered for linkedlist of dragged cards
        PicBox.MouseMove += (sender, e) => 
        {
            //changed this to grab from the linkedlist version of CurDragCards
            if (FrmGame.CurDragCards.Contains(this))
            {
                    var dragged = (Control)sender;
                    Point screenPos = dragged.PointToScreen(e.Location);
                    Point parentPos = dragged.Parent.PointToClient(screenPos);
                    dragged.Left = screenPos.X - dragOffset.X;
                    dragged.Top = screenPos.Y - dragOffset.Y;

                    // Find the control currently under the mouse
                    Control target = FrmGame.Instance.GetChildAtPoint(dragged.Parent.PointToClient(screenPos));

                    // Avoid detecting the dragged control itself
                    if (target is not null && target != dragged)
                    {
                        var dropTarget = Game.FindDropTarget(target); //find the control underneath the card
                        if (dropTarget is null)
                        {
                            Game.CallDragEndedOnAll();  //card is hovering over nothing, remove highlights
                        }
                        //card has moved from target
                        else if (dropTarget != lastDropTarget)
                        {
                            lastDropTarget?.DragEnded(); //remove the highlight from the previous target
                        }
                        //card has dragged over a new target
                        if (dropTarget != FrmGame.CardsDraggedFrom as IDropTarget)
                        {
                            dropTarget?.DragOver(this); //call targets drag functons, highlight
                            lastDropTarget = dropTarget; //set this target to last target
                        }
                    }

                    //redraw the card
                    dragged.Location = new Point(
                        parentPos.X - dragOffset.X,
                        parentPos.Y - dragOffset.Y
                    );
                
            }
        };
    }

    public void FlipOver()
    {
        FaceUp = !FaceUp;
        PicBox.BackgroundImage = PicImg;
    }

    public void AdjustLocation(int left, int top)
    {
        PicBox.Left = left;
        PicBox.Top = top;
    }

    /// <summary>
    /// Updates the card's displayed image to match its current state
    /// </summary>
    public void RefreshImage()
    {
        PicBox.BackgroundImage = PicImg;        // Update the PictureBox to show the current card image
    }
}

public class TableauStack : IFindMoveableCards, IDropTarget, IDragFrom
{
    public Panel Panel { get; set; }
    public LinkedList<Card> Cards { get; private set; }

    public TableauStack(Panel panel)
    {
        Panel = panel;
        Cards = new();
    }

    public void AddCard(Card c)
    {
        //check first to see if its just the wildcard in the tableau
        if(Cards.Count == 1 && Cards.First.Value.IsWildCard == true)
        {
            Cards.First.Value.Type = c.Type + 1; //set wildcards type to be one higher than the card added below it
            Cards.First.Value.Suit = (Suit)(((int)c.Suit % 2) + 1); //suit is enum so cast to int, mod and add one so suit of wildcard will be different than new card, then re-cast to suit. Doesnt matter what suit as long as different color. //dont know whether to be proud or horrified of the right side.
        }
        c.IsIn = this; //set what tableau stack the card is in

        // handle the wildcard differently
        if (c.IsWildCard)
        {
            //if the tableau stack is not empty, i.e. adding beneath a card
            if(Cards.Count != 0)
            {

                c.Type = c.IsIn.Cards.Last.Value.Type - 1; //set the type to be one less than the card above
                if ((int)c.IsIn.Cards.Last.Value.Suit % 2 == 0)
                {
                    c.Suit = Suit.SPADES;
                }
                else 
                {
                    c.Suit = Suit.HEARTS;
                }
            }
        }
        Cards.AddLast(c);
        Panel.AddCard(c);
        c.PicBox.BringToFront();
    }
    /// <summary>
    /// return a list of movable cards
    /// </summary>
    /// <returns>[] if no movable cards</returns>
    public List<Card> FindMoveableCards()
    {
        List<Card> cards = new();
        //if tableau not empty
        if (Cards.Count != 0)
        {
            //iterate through tableaus cards and add moveable one to list
            foreach (Card c in Cards)
            {
                if (c.FaceUp)
                {
                    cards.Add(c);
                }
            }
        }

        return Cards.Count > 0 ? cards : []; //return list or empty list if null+
    }
    
    //highlights the control if dragged over
    public void DragOver(Card c)
    {
        if (CanDrop(c))
        {
            Panel.BackColor = Color.Green;
        }
        else
        {
            Panel.BackColor = Color.Red;
        }
    }
    
    /// <summary>
    /// Checks to see if card is valid to be dropped into Tableau stack
    /// </summary>
    /// <param name="c"> c is the card user is holding </param>
    /// <returns>bool</returns>
    public bool CanDrop(Card c) {
        if (c.IsWildCard)
        {
            //make sure the tableau isnt empty to avoid null point exception
            if (Cards.Last != null)
            {
                // If there's a card underneath the Wild Card OR card underneath is ACE it can no longer be moved to any tableau stack
                if (FrmGame.CurDragCards.Count > 1 || Cards.Last.Value.Type == CardType.ACE)
                {
                    //return false if the card that the wild will be placed under isnt the 1 type greater or is the same color
                    if (Cards.Last.Value.Type != c.Type + 1 || (int) Cards.Last.Value.Suit % 2 == (int) c.Suit % 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //tableau is empty the only valid cards are king or wild
        if (Cards.Count == 0) {
            return c.Type == CardType.KING;
        }
        //allow dropping for lone wildcard in tableau
        else if(Cards.Count == 1 && Cards.First.Value.IsWildCard == true) 
        {
            return true;
        }
        //otherwise have to check if card is valid to be placed under target card
        else
        {
            Card lastCard = Cards.Last.Value;
            bool suitCheck = ((int)lastCard.Suit % 2 != (int)c.Suit % 2);
            bool typeCheck = lastCard.Type == c.Type + 1;
            return (suitCheck && typeCheck);
        }
    }
    
    //card is dropped on tableau
    public void Dropped(Card c) {
        AddCard(c);
        FrmGame.Instance.RemCard(c);
        Panel.AddCard(c);

        // If C is coming from Talon stack, reward player 10 points
        if (FrmGame.CardsDraggedFrom is Talon)
        { 
            ScoreManager.AddPoints(10);         
        }

        c.AdjustLocation(0, (Cards.Count - 1) * 20); //set the new location underneath the last card in the stack
        c.PicBox.BringToFront();
        Panel.Refresh();
        c.PicBox.BringToFront();

    }

    //gets rid of highlights after no longer being dragged over
    public void DragEnded()
    {
        Panel.BackColor = Color.Transparent;
    }

    public Card GetBottomCard()
    {
        return Cards.Count > 0 ? Cards.Last.Value : null;
    }

    public void RemCard(Card card)
    {
        Cards.Remove(card);
    }
}

public class Talon : IFindMoveableCards, IDragFrom
{
    public Panel Panel { get; private set; }
    public Stack<Card> Cards { get; private set; }

    public Talon(Panel pan)
    {
        Panel = pan;
        Cards = new();
    }

    /// <summary>
    /// Puts cards from the talon back into the deck
    /// </summary>
    /// <param name="deck"></param>
    public void ReleaseIntoDeck(Deck deck)
    {
        //start at the back of Cards so that the order of the deck is maintained
        foreach (var card in Cards.Reverse())
        {
            deck.Release(card);
            Panel.RemCard(card);
        }
        Cards.Clear();
    }

    public void AddCard(Card c)
    {
        Cards.Push(c);
        Panel.AddCard(c);
    }

    public List<Card> FindMoveableCards() => (Cards.Count > 0 ? [Cards.Peek()] : []);

    public void RemCard(Card card)
    {
        if (Cards.Peek() == card)
        {
            Cards.Pop();
        }
    }
}

public class FoundationStack : IFindMoveableCards, IDropTarget, IDragFrom
{
    public Panel Panel { get; private set; }
    public Stack<Card> Cards { get; private set; }
    public Suit Suit { get; private init; }

    public FoundationStack(Panel panel, Suit suit)
    {
        Panel = panel;
        Cards = new();
        Suit = suit;
    }

    public List<Card> FindMoveableCards() => (Cards.Count > 0 ? [Cards.Peek()] : []);

    public void DragOver(Card c)
    {
        if (CanDrop(c))
        {
            Panel.BackColor = Color.Green;
        }
        else
        {
            Panel.BackColor = Color.Red;
        }
    }

    /// <summary>
    /// Determines if a card can be placed into the foundation stack.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool CanDrop(Card c)
    {
        // if card is a Wild Card, it can't drop into foundation stack
        if (c.IsWildCard)
        {
            Panel.BackColor = Color.Red;
            return false;
        }

        Card topCard = Cards.Count > 0 ? Cards.Peek() : null;
        bool suitCheck;
        bool typeCheck;

        suitCheck = Suit == c.Suit; //if card's suit matches the foundation's suit, true or false
        if (topCard is null)
        {
            typeCheck = c.Type == CardType.ACE; //if card's type matches the only valid type, true or false
        }
        else
        {
            typeCheck = topCard.Type == c.Type - 1; //card being added must be one greater than the top card
        }
        if (typeCheck && suitCheck)
        {
            Panel.BackColor = Color.Green;
        }
        else
        {
            Panel.BackColor = Color.Red;
        }
        return suitCheck && typeCheck;
        
    }

    public void Dropped(Card c)
    {
        Cards.Push(c);
        FrmGame.Instance.RemCard(c);
        Panel.AddCard(c);

        ScoreManager.AddPoints(20);             // Player earns 20 points for dropping cards into foundation stacks
        c.AdjustLocation(0, 0);
        c.PicBox.BringToFront();
        c.IsIn = null; //card is no longer in a tableau stack
        Game.checkWin(); //after placed check to see if board is empty
        //c.Foundation = this;
    }

    public void DragEnded()
    {
        Panel.BackColor = Color.Transparent;
    }

    public void RemCard(Card card)
    {
        if (Cards.Count > 0 && Cards.Peek() == card)
            Cards.Pop();
    }

    public void AddCard(Card card)
    {
        Dropped(card);
    }
}

public static class Game
{
    public static Form TitleForm { get; set; } //grab the instance for the title form
    public static Form RoundForm { get; set; } //grab the instance for the round form
    public static Deck Deck { get; private set; }
    public static Dictionary<Suit, FoundationStack> FoundationStacks { get; set; }
    public static TableauStack[] TableauStacks;
    public static Talon Talon { get; set; }
    public static int StockReloadCount { get; set; } //counter for how many times the deck has been refreshed
    public static Card WildCard = null; //flag for if a wildcard is in play


    static Game()
    {
        StockReloadCount = 0;
    }

    /// <summary>
    /// Initialises the game's structures, and deals cards.
    /// </summary>
    /// <param name="panTalon"></param>
    /// <param name="panTableauStacks"></param>
    /// <param name="panFoundationStacks"></param>
    public static void Init(Panel panTalon, Panel[] panTableauStacks, Dictionary<Suit, Panel> panFoundationStacks)
    {
        Deck = new();
        WildCard = null;

        // create talon
        Talon = new(panTalon);

        // create tableau stacks
        TableauStacks = new TableauStack[7];
        for (int i = 0; i < TableauStacks.Length; i++)
        {
            TableauStacks[i] = new(panTableauStacks[i]);
        }

        // create foundation stacks
        FoundationStacks = new();
        foreach (var suit in Enum.GetValues<Suit>())
        {
            FoundationStacks.Add(suit, new(panFoundationStacks[suit], suit));
        }

        // load tableau stacks
        const int VERT_OFFSET = 20;
        for (int i = 0; i < TableauStacks.Length; i++)
        {
            Card c;
            //based on what tableau it is, add the required amount of cards
            for (int j = 0; j < i; j++)
            {
                c = Deck.Acquire();
                c.FlipOver();
                c.AdjustLocation(0, j * VERT_OFFSET);
                TableauStacks[i].AddCard(c);
            }

            //dont flip the last card
            c = Deck.Acquire();
            c.AdjustLocation(0, i * VERT_OFFSET);
            TableauStacks[i].AddCard(c);
        }
    }

    /// <summary>
    /// Iterates through the game's structures to see if the card is contained in their list of moveable cards.
    /// </summary>
    /// <param name="c"></param>
    /// <returns>true or false</returns>
    public static bool IsCardMovable(Card c)
    {
        bool isMovable = false;
        isMovable |= Talon.FindMoveableCards().Contains(c);
        foreach (var foundationStack in FoundationStacks)
        {
            isMovable |= foundationStack.Value.FindMoveableCards().Contains(c);
        }
        foreach (var tableauStack in TableauStacks)
        {
            isMovable |= tableauStack.FindMoveableCards().Contains(c);
        }
        return isMovable;
    }

    /// <summary>
    /// Find where the card is being dragged from.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IDragFrom FindDragFrom(Card c)
    {
        if (Talon.Cards.Contains(c))
        {
            return Talon;
        }
        foreach (var foundationStack in FoundationStacks)
        {
            if (foundationStack.Value.Cards.Contains(c))
            {
                return foundationStack.Value;
            }
        }
        foreach (var tableauStack in TableauStacks)
        {
            if (tableauStack.Cards.Contains(c))
            {
                return tableauStack;
            }
        }
        return null;
    }

    /// <summary>
    /// Find which of the game's structures match the control.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IDropTarget FindDropTarget(Control c)
    {
        foreach (var foundationStack in FoundationStacks)
        {
            if (foundationStack.Value.Panel == c)
            {
                return foundationStack.Value;
            }
        }
        foreach (var tableauStack in TableauStacks)
        {
            if (tableauStack.Panel == c)
            {
                return tableauStack;
            }
        }
        return null;
    }

    /// <summary>
    /// Calls all structure DragEnded() methods to eliminate lingering highlights.
    /// </summary>
    public static void CallDragEndedOnAll()
    {
        foreach (var foundationStack in FoundationStacks)
        {
            foundationStack.Value.DragEnded();
        }
        foreach (var tableauStack in TableauStacks)
        {
            tableauStack.DragEnded();
        }
    }

    //if card does not have a card below it then return true
    public static bool CanFlipOver(Card c)
    {
        foreach (var tableauStack in TableauStacks)
        {
            if (tableauStack.GetBottomCard() == c)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if all cards are in the foundations to determine if win is achieved.
    /// </summary>
    public static void checkWin()
    {
        foreach (TableauStack t in TableauStacks)
        {
            if (t.Cards.Count != 0)
            {
                return;
            }
        }

        if (Deck.cards.Count != 0 || Talon.Cards.Count != 0)
        {
            return;
        }

        FrmGame.EndOfRound(true); //opens the end of round form
    }

    /// <summary>
    /// Causes all cards to drift across the screen at varying speeds.
    /// </summary>
    public static void Explode()
    {
        List<Card> allCardsInPlay = new();
        foreach (var foundationStack in FoundationStacks)
        {
            allCardsInPlay.AddRange(foundationStack.Value.Cards);
        }
        foreach (var tableauStack in TableauStacks)
        {
            allCardsInPlay.AddRange(tableauStack.Cards);
        }
        allCardsInPlay.AddRange(Talon.Cards);
        foreach (Card c in allCardsInPlay)
        {
            Point origPos = c.PicBox.Location;
            origPos.X += c.PicBox.Parent.Location.X;
            origPos.Y += c.PicBox.Parent.Location.Y;
            c.PicBox.Parent.RemCard(c);
            FrmGame.Instance.AddCard(c);
            c.AdjustLocation(origPos.X, origPos.Y);
            c.PicBox.BringToFront();
        }
        const int SPEED = 6;
        const int MORE_SPEED = 10;
        Point[] possibleExplodeVectors = [
            new(0, SPEED),
            new(0, -SPEED),

            new(SPEED, 0),
            new(-SPEED, 0),

            new(SPEED, SPEED),
            new(-SPEED, SPEED),

            new(SPEED, -SPEED),
            new(-SPEED, -SPEED),

            new(SPEED, MORE_SPEED),
            new(-SPEED, MORE_SPEED),
            new(SPEED, -MORE_SPEED),
            new(-SPEED, -MORE_SPEED),

            new(MORE_SPEED, SPEED),
            new(-MORE_SPEED, SPEED),
            new(MORE_SPEED, -SPEED),
            new(-MORE_SPEED, -SPEED),
        ];
        Point[] explodeVectors = new Point[allCardsInPlay.Count];
        Random rand = new();
        for (int i = 0; i < explodeVectors.Length; i++)
        {
            explodeVectors[i] = possibleExplodeVectors[rand.Next(possibleExplodeVectors.Length)];
        }
        Timer tmr = new() { Interval = 25 };
        tmr.Tick += (sender, e) => {
            for (int i = 0; i < allCardsInPlay.Count; i++)
            {
                Card c = allCardsInPlay[i];
                c.AdjustLocation(c.PicBox.Location.X + explodeVectors[i].X, c.PicBox.Location.Y + explodeVectors[i].Y);
            }
        };
        tmr.Start();
    }

    /// <summary>
    /// Spawns a single Wild Card into the game if one doesn't already exist.
    /// The Wild Card is a special card that can be dragged to any tableau stack. 
    /// </summary>
    public static void SpawnWildCard()
    {
        // Prevent multiple wild cards
        if (WildCard != null)
            return;

        // Create a new card - type/suit doesn't matter
        Card wild = new Card(CardType.ACE, Suit.SPADES)
        {
            IsWildCard = true
        };

        wild.RefreshImage();        // Update the card's image 

        Talon.AddCard(wild);        // Add the card to the Talon to ensure it's properly parented and draggable

        wild.PicBox.BringToFront();

        WildCard = wild;

        MessageBox.Show("Wild Card spawned! Drag it to any tableau stack.", "Crazy Solitaire");     // Notify the player that the Wild Card has been spawned
    }

}

