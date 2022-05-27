using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BlackJack
{
    public partial class MainForm : Form
    {
        private string Dealer ;
        private string Player ;
        private int HighestPoint = 0;
        private int BlackJackWinningPoint = 0;
        private int NormalWinningPoint = 0;
        private int CardHeight = 0;
        private double DealerDrawPercentage = 0;
        private int MaximumHandCard = 0;
        private int MinimumCardNumber = 0;
        private int DealerMinimumPoint = 0;
        System.Windows.Forms.Timer timer1; 

        List<Card> TrashCards = new List<Card>();   //To store the used card. 
        List<User> users = new List<User>();
        CardController Cardcontroller;

//Cnofirm pushed
        public MainForm(NameValueCollection appSettings)
        {
            try
            {
                // Get Configuration From AppSetting
                Dealer = appSettings.Get("DealerName");
                Player = appSettings.Get("PlayerName");
                int.TryParse(appSettings.Get("HighestPoint") , out HighestPoint);
                int.TryParse(appSettings.Get("BlackJackWinningPoint"), out BlackJackWinningPoint);
                int.TryParse(appSettings.Get("NormalWinningPoint"), out NormalWinningPoint);
                int.TryParse(appSettings.Get("CardHeight") , out CardHeight);
                int.TryParse(appSettings.Get("MaximumHandCard"), out MaximumHandCard);
                int.TryParse(appSettings.Get("MinimumCardNumber"), out MinimumCardNumber);
                int.TryParse(appSettings.Get("DealerMinimumPoint"), out DealerMinimumPoint);
                double.TryParse(appSettings.Get("DealerDrawPercentage"), out DealerDrawPercentage);


                InitializeComponent();
                Cardcontroller = new CardController(MinimumCardNumber);
                Cardcontroller.Shuffle(30);

                users.Add(new User()         //Initialize with two users 
                {
                    Name = Dealer,
                    HandCard = new List<Card>(),
                    Point = 0
                });
                users.Add(new User()
                {
                    Name = Player,
                    HandCard = new List<Card>(),
                    Point = 0
                });

                StartNewRound(null, null);
            }catch(Exception ex)
            {
                new Logging().WriteLog(ex.Message);
            }
        }

        private void UpdateUI(bool ShowDealer = false)
        {
            try
            {
                User player = users.Where(usr => usr.Name.Equals(Player)).FirstOrDefault();
                User dealer = users.Where(usr => usr.Name.Equals(Dealer)).FirstOrDefault();
                label2.Text = player?.Point.ToString();
                label4.Text = dealer?.Point.ToString();
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();
                foreach (Card card in player?.HandCard)
                {
                    flowLayoutPanel1.Controls.Add(new Button()
                    {
                        Name = "Btton",
                        Text = GetDisplayNumeric(card),
                        Height = CardHeight
                    });
                }
                foreach (Card card in dealer?.HandCard)
                {
                    flowLayoutPanel2.Controls.Add(new Button()
                    {
                        Name = "Btton",
                        Text = ShowDealer ? GetDisplayNumeric(card) : "?",
                        Height = CardHeight
                    });
                }
            }catch(Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private string GetDisplayNumeric(Card card)
        {
            try
            {
                string unicode = string.Empty;
                switch (card.suits)
                {
                    case Suits.Spade:
                        {
                            unicode = "\u2660";
                            break;
                        }
                    case Suits.Heart:
                        {
                            unicode = "\u2665";
                            break;
                        }
                    case Suits.Club:
                        {
                            unicode = "\u2663";
                            break;
                        }
                    case Suits.Diamond:
                        {
                            unicode = "\u2666";
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                return unicode + " " + card.Numeric;
            }catch(Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private void MatchCalculator()
        {
            try
            {
                //to check whether the dealer win or player win the game 
                User dealer = users.Where(usr => usr.Name.Equals(Dealer)).FirstOrDefault();
                int DealerPoint = dealer == null ? 0 : GetSum(dealer?.HandCard);
                foreach (User player in users.Where(usr => !usr.Name.Equals(Dealer)).ToList())
                {
                    int PlayerPoint = GetSum(player.HandCard);
                    if (PlayerPoint == DealerPoint || (PlayerPoint > HighestPoint && DealerPoint > HighestPoint))
                    {
                        continue;
                    }
                    if (PlayerPoint > DealerPoint && PlayerPoint <= HighestPoint || DealerPoint > HighestPoint && PlayerPoint <= HighestPoint)
                    {
                        player.Point += (PlayerPoint == HighestPoint ? BlackJackWinningPoint : NormalWinningPoint);
                        continue;
                    }
                    if (DealerPoint > PlayerPoint && DealerPoint <= HighestPoint || PlayerPoint > HighestPoint && DealerPoint <= HighestPoint)
                    {
                        dealer.Point += (DealerPoint == HighestPoint ? BlackJackWinningPoint : NormalWinningPoint);
                        continue;
                    }

                }
            }
            catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private int GetSum(List<Card> cards)
        {
            try
            {
                int TotalSum = 0;
                foreach (Card card in cards)
                {
                    int Sum = 0;
                    if (!int.TryParse(card.Numeric, out Sum))
                    {
                        if (card.Numeric.ToUpper() != "A")
                        {
                            //J or Q or K
                            Sum = 10;
                        }
                        else
                        {
                            //is A
                            Sum = 1;
                        }
                    }
                    TotalSum += Sum;
                }
                if (cards.Where(card => card.Numeric.ToUpper() == "A" && card.suits.Equals(Suits.Spade)).FirstOrDefault() != null)
                {
                    // Special Condition for spade A 
                    if(TotalSum + 10 <= HighestPoint)
                    {
                        TotalSum += 10;
                    }
                }
                return TotalSum;
            }
            catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private void RoundEnd()
        {
            try
            {
                UpdateUI(true);
                MatchCalculator();
                timer1 = new System.Windows.Forms.Timer();
                timer1.Tick += new EventHandler(StartNewRound);
                timer1.Interval = 3000;
                timer1.Start();
            }
            catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private void StartNewRound(object sender, EventArgs e)
        {
            try
            {
                timer1?.Stop();
                foreach (User usr in users)
                {
                    TrashCards.AddRange(usr.HandCard);      //move all used card to trash card list 
                    usr.HandCard = new List<Card>();        //remove all hand card 
                }
                for (int r = 0; r < 2; r++)
                {
                    //Each player and banker draw two cards
                    users.ForEach(user => user.HandCard.Add(Cardcontroller.DrawCard()));
                }
                UpdateUI();
            }
            catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
        private void DrawCard(string Round)
        {
            try
            {
                Cardcontroller.Reshuffle(TrashCards, out TrashCards);
                users.Find(usr => usr.Name.Equals(Round) && usr.HandCard.Count < MaximumHandCard)?.HandCard.Add(Cardcontroller.DrawCard());
                UpdateUI();
            }
            catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Player Draw card 
            DrawCard(Player);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Dealer's Turn 
            try
            {
                Random random = new Random();
                User dealer = users.Where(usr => usr.Name.Equals(Dealer)).FirstOrDefault();
                while (GetSum(dealer.HandCard) < HighestPoint)
                {
                    bool drawed = false;
                    
                    if (random.NextDouble() * 100 <= DealerDrawPercentage || GetSum(dealer.HandCard) < DealerMinimumPoint)
                    {
                        DrawCard(Dealer);
                        drawed = true;
                    }
                    if (!drawed)
                    {
                        break;          // break if the dealer stop hit 
                    }
                }
                RoundEnd();
            }catch (Exception ex)
            {
                new Logging().WriteLog(ex.Message);
                throw;
            }
        }
    }
}
