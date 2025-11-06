using CrazySolitaire.Properties;
using System;
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
    public bool IsWildCard { get; set; } = false;
    public bool HasScoredInFoundation { get; set; } = false;
    public PictureBox PicBox { get; private set; }
    internal TableauStack IsIn { get; set; }//tableau stack that card is currently in
    // internal FoundationStack Foundation {  get; set; }//foundation stack that card is currently in

    public Bitmap PicImg
    {
        get
        {
            if (!FaceUp)
                return Resources.back_green;

            if (IsWildCard)
                return Resources.wild_card; 

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
                ScoreManager.AddPoints(5);
            }
        };

        PicBox.MouseDown += (sender, e) => {
             if (e.Button == MouseButtons.Left && Game.IsCardMovable(this))
            {
                //changed to handle dragging multiple valid cards
                //if in a tableau stack
                if (this.IsIn != null)
                {
                    //a list of all the cards that are draggable
                    List<Card> movables = this.IsIn.FindMoveableCards();
                    //find the point in the stack that is being dragged
                    int index = movables.IndexOf(this);

                    //drag all the other cards
                    while (index < movables.Count)
                    {
                        if(index == 0)
                        {
                            FrmGame.DragCard(movables[index]);
                            movables[index].dragOffset = e.Location;
                            movables[index].conBeforeDrag = movables[index].PicBox.Parent;
                            movables[index].relLocBeforeDrag = movables[index].PicBox.Location;
                            conBeforeDrag.RemCard(movables[index]);
                            FrmGame.Instance.AddCard(movables[index]);
                            Point loc = movables[index].conBeforeDrag.Location;
                            movables[index].PicBox.Location = new Point(loc.X, loc.Y + (relLocBeforeDrag.Y));
                            movables[index].PicBox.BringToFront();
                            index++;
                        }
                        else
                        {
                            FrmGame.DragCard(movables[index]);
                            movables[index].dragOffset = e.Location;
                            movables[index].conBeforeDrag = movables[index].PicBox.Parent;
                            movables[index].relLocBeforeDrag = movables[index].PicBox.Location;
                            conBeforeDrag.RemCard(movables[index]);
                            FrmGame.Instance.AddCard(movables[index]);
                            Point loc = movables[index].conBeforeDrag.Location;
                            movables[index].PicBox.Location = new Point(1000,1000);
                            movables[index].PicBox.BringToFront();
                            index++;
                        }
                        
                    }
                }
                else
                {
                    //otherwise card is coming from talon or foundation, indifferent from original code
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
        PicBox.MouseUp += (sender, e) => {
            if (FrmGame.CurDragCards.Contains(this))//altered from original to work with linkedlist
            {
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
                Game.CallDragEndedOnAll();
            }
        };

        //may need to look at this to fix cards in top right
        //relLocBeforeDrag is just where the cursor was on the original control, not scaled for whole screen
        PicBox.MouseMove += (sender, e) => 
        {
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
                        var dropTarget = Game.FindDropTarget(target);
                        if (dropTarget is null)
                        {
                            Game.CallDragEndedOnAll();
                        }
                        else if (dropTarget != lastDropTarget)
                        {
                            lastDropTarget?.DragEnded();
                        }
                        if (dropTarget != FrmGame.CardsDraggedFrom as IDropTarget)
                        {
                            dropTarget?.DragOver(this);
                            lastDropTarget = dropTarget;
                        }
                    }

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

    public void RefreshImage()
    {
        PicBox.BackgroundImage = PicImg;
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
            //if so set the wildcard to validate the new cards beneath it
            Cards.First.Value.Type = c.Type + 1;
            Cards.First.Value.Suit = (Suit)(((int)c.Suit % 2) + 1); //suit is enum so cast to int, mod and add one so suit of wildcard will be different than new card, then re-cast to suit
        }
        c.IsIn = this;

        if (c.IsWildCard)
        {
            if(Cards.Count != 0)
            {
                c.Type = c.IsIn.Cards.Last.Value.Type - 1;
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

    public List<Card> FindMoveableCards()
    {
        List<Card> cards = new();
        if (Cards.Count != 0)
        {
            foreach (Card c in Cards)
            {
                if (c.FaceUp)
                {
                    cards.Add(c);
                }
            }
        }

        return Cards.Count > 0 ? cards : [];
    }

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
    /// <returns></returns>
    public bool CanDrop(Card c) {
        if (c.IsWildCard)
        {
            //make sure the tableau isnt empty to avoid null point exception
            if (Cards.Last != null)
            {
                // If there's a card underneath the Wild Card OR card underneath is ACE it can no longer be moved to any tableau stack
                if (FrmGame.CurDragCards.Count > 1 || Cards.Last.Value.Type == CardType.ACE)
                {
                    // If the last card in the Tableau stack is valid for the WildCard current type & suit
                    if (Cards.Last.Value.Type != c.Type + 1 || (int) Cards.Last.Value.Suit % 2 == (int) c.Suit % 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        if (Cards.Count == 0) {
            return c.Type == CardType.KING;
        }
        else if(Cards.Count == 1 && Cards.First.Value.IsWildCard == true) //allow dropping for lone wildcard in tableau
        {
            return true;
        }
        else
        {
            Card lastCard = Cards.Last.Value;
            bool suitCheck = ((int)lastCard.Suit % 2 != (int)c.Suit % 2);
            bool typeCheck = lastCard.Type == c.Type + 1;
            return (suitCheck && typeCheck);
        }
    }

    public void Dropped(Card c) {
        AddCard(c);
        FrmGame.Instance.RemCard(c);
        Panel.AddCard(c);

        if (FrmGame.CardsDraggedFrom is Talon)
        { 
            ScoreManager.AddPoints(10);         
        }

        c.AdjustLocation(0, (Cards.Count - 1) * 20);
        c.PicBox.BringToFront();
        Panel.Refresh();
        c.PicBox.BringToFront();

    }

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

    public void ReleaseIntoDeck(Deck deck)
    {
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

    public bool CanDrop(Card c)
    {
        if (c.IsWildCard)
        {
            Panel.BackColor = Color.Red;
            return false;
        }

        Card topCard = Cards.Count > 0 ? Cards.Peek() : null;
        bool suitCheck;
        bool typeCheck;

        suitCheck = Suit == c.Suit;
        if (topCard is null)
        {
            typeCheck = c.Type == CardType.ACE;
        }
        else
        {
            typeCheck = topCard.Type == c.Type - 1;
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

        ScoreManager.AddPoints(20);
        c.AdjustLocation(0, 0);
        c.PicBox.BringToFront();
        c.IsIn = null;
        Game.checkWin();
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
    public static Form TitleForm { get; set; }
    public static Form RoundForm { get; set; }
    public static Deck Deck { get; private set; }
    public static Dictionary<Suit, FoundationStack> FoundationStacks { get; set; }
    public static TableauStack[] TableauStacks;
    public static Talon Talon { get; set; }
    public static int StockReloadCount { get; set; }
    public static Card WildCard = null;


    static Game()
    {
        StockReloadCount = 0;
    }

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
            for (int j = 0; j < i; j++)
            {
                c = Deck.Acquire();
                c.FlipOver();
                c.AdjustLocation(0, j * VERT_OFFSET);
                TableauStacks[i].AddCard(c);
            }
            c = Deck.Acquire();
            c.AdjustLocation(0, i * VERT_OFFSET);
            TableauStacks[i].AddCard(c);
        }
    }

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

        FrmGame.EndOfRound(true);
    }
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

    public static void SpawnWildCard()
    {
        // only one at a time
        if (WildCard != null)
            return;

        // Create a new card (the type/suit don't really matter visually)
        Card wild = new Card(CardType.ACE, Suit.SPADES)
        {
            IsWildCard = true
        };
        wild.RefreshImage();

        Talon.AddCard(wild); // ensures proper parent and drag registration

        wild.PicBox.BringToFront();

        // Store a reference
        WildCard = wild;

        MessageBox.Show("Wild Card spawned! Drag it to any tableau stack.", "Crazy Solitaire");
    }

}

