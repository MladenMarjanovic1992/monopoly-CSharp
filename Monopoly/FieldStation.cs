﻿using System;
using System.Collections.Generic;

namespace Monopoly
{
    public class FieldStation : IFieldRentable
    {
        public string FieldName { get; set; }
        public int FieldIndex { get; set; }

        public Player Owner { get; set; }
        public int Price { get; set; }
        public bool UnderMortgage { get; set; }
        public int[] Rent { get; set; }
        public int CurrentRent { get; set; }
        public string Color { get; set; }
        public bool CanMortgage { get; set; }
        public bool CanTrade { get; set; }

        public FieldStation()
        {
            Rent = new int[4];
            CanMortgage = true;
            CanTrade = true;
        }

        public int MortgageValue => Price / 2;

        public void FieldEffect(Player currentPlayer, List<Player> otherPlayers)
        {
            PrintFieldStats();

            // Rule: Player must pay rent for an owned field when he lands on it
            if (Owner != null && Owner != currentPlayer && !UnderMortgage)
            {
                currentPlayer.Money -= CurrentRent;
                Owner.Money += CurrentRent;

                Console.WriteLine($"Payed: {CurrentRent}");
            }
            else if (Owner == null)
            {
                var buysField = Prompt.YesOrNo("Buy field? (y/n)") && currentPlayer.Money >= Price;

                // Rule: Player can buy unowned field when he lands on it
                if (buysField)
                {
                    var trade = new Trade();
                    trade.BuyField(currentPlayer, this, Price);
                }
                else
                {
                    Console.WriteLine("Auction!");

                    foreach (var player in otherPlayers) // So the players know how much they can bid
                    {
                        player.PrintStats();
                    }

                    // Rule: When the player can't afford a field, or doesn't want to buy it, the field is auctioned to other players
                    var trade = new Trade();
                    var highestBidder = Prompt.ChoosePlayer(otherPlayers, "Which player had the highest bid (enter number)?");
                    var highestBid = Prompt.EnterAmount(highestBidder, "What was the winning bid?");

                    trade.BuyField(highestBidder, this, highestBid);
                }
            }
        }

        public void PrintFieldStats()
        {
            Console.WriteLine();
            Console.WriteLine($"Field: {FieldName}");
            Console.WriteLine($"Field position: {FieldIndex}");
            Console.WriteLine($"Price: {Price}");
            Console.WriteLine($"Mortgage value: {MortgageValue}");
            Console.Write("Rent (/1/2/3/4/): ");
            foreach (var i in Rent)
            {
                Console.Write($"/{i}");
            }
            Console.WriteLine();
            Console.WriteLine("Owner: {0}", Owner == null ? "Not owned" : Owner.PlayerName);
            Console.WriteLine($"Current rent (0 if not owned): {CurrentRent}");
            Console.WriteLine("Under mortgage: {0}", UnderMortgage ? "Yes" : "No");
            Console.WriteLine();
        }
    }
}