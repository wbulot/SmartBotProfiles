//Smarbot Profile
//Deck from :
//Contributors : Wbulot

using System.Linq;
using System.Collections.Generic;

namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {

        private int MinionEnemyTauntValue = 2;
        private int MinionEnemyWindfuryValue = 2;
        private int MinionDivineShield = 2;

        private int FriendCardDrawValue = 2;
        private int EnemyCardDrawValue = 2;

        private int HeroEnemyHealthValue = 2;
        private int HeroFriendHealthValue = 2;

        private int MinionEnemyAttackValue = 3;
        private int MinionEnemyHealthValue = 2;
        private int MinionFriendAttackValue = 2;
        private int MinionFriendHealthValue = 2;

        //GlobalValueModifier
        private int GlobalValueModifier = 0;

        public override float GetBoardValue(Board board)
        {
            float value = 0;

            //Hero friend value
            value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;

            //Hero enemy value
            value -= (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) * HeroEnemyHealthValue;

            //enemy board
            foreach (Card c in board.MinionEnemy)
            {
                value -= GetCardValue(board, c);
            }

            //friend board
            foreach (Card c in board.MinionFriend)
            {
                value += GetCardValue(board, c);
            }

            //Lethal and save my ass
            if (board.HeroEnemy.CurrentHealth <= 0)
                value += 100000;

            if (board.HeroFriend.CurrentHealth <= 0 && board.FriendCardDraw == 0)
                value -= 100000;

            value += GlobalValueModifier;

            value += board.FriendCardDraw * FriendCardDrawValue;
            value -= board.EnemyCardDraw * EnemyCardDrawValue;

            return value;
        }

        //public override void OnBoardReady(Board board)
        //{
        //    if (board.IsOwnTurn)
        //    {
        //        foreach (var item in board.ActionsStack)
        //        {
        //            Debug(item.ToString());
        //        }

        //        Debug("Board : " + board.GetValue());
        //    }
        //}

        public float GetCardValue(Board board, Card card)
        {
            float value = 0;

            if (card.IsFriend)
            {
                value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 5;

                //Tweak some minions Friend value
                switch (card.Template.Id)
                {
                    case Card.Cards.FP1_007://Nerubian Egg
                        if (card.IsTaunt)
                        {
                            value += 5;
                        }
                        break;
                }
            }
            else
            {
                //Taunt value
                if (card.IsTaunt)
                    value += MinionEnemyTauntValue;

                //Windfury value
                if (card.IsWindfury)
                    value += MinionEnemyWindfuryValue;

                //divine shield value
                if (card.IsDivineShield)
                    value += MinionDivineShield;

                value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;

                //Tweak some minions enemy value
                switch (card.Template.Id)
                {
                    case Card.Cards.EX1_412://Raging Worgen

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_565://Flametongue Totem

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.BRM_002://Flamewaker

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_402://Armorsmith

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_006://Mechwarper

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.FP1_005://Shade of Naxxramas

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.FP1_013://Kel'Thuzad

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.BRM_031://Chromaggus

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_559://Archmage Antonidas

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_021://Mal'Ganis

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_608://Sorcerer's Apprentice

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_012://Mana Wyrm

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_595://Cult Master

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_170://Emperor Cobra

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.BRM_028://Emperor Thaurissan

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_100://Floating Watcher

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.tt_004://Flesheating Ghoul

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_604://Frothing Berserker

                        if (card.IsSilenced == false)
                        {
                            value += 20;
                        }
                        break;

                    case Card.Cards.BRM_019://Grim Patron

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_084://Warsong Commander

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_095://Gadgetzan Auctioneer

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_040://Hogger

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_104://Hobgoblin

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_614://Illidan Stormrage

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_027://Iron Sensei

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_094://Jeeves

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_019://Knife Juggler

                        if (card.IsSilenced == false)
                        {
                            value += 9;
                        }
                        break;

                    case Card.Cards.EX1_001://Lightwarden

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_563://Malygos

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_103://Micro Machine

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_044://Questing Adventurer

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_020://Wild Pyromancer

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_013://Cogmaster

                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.CS2_237://Starving Buzzard

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_080://Secretkeeper

                        if (card.IsSilenced == false)
                        {
                            value += 7;
                        }
                        break;

                    case Card.Cards.GVG_002://Snowchugger

                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;
                }
            }

            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {

        }

        public override void OnMinionDeath(Board board, Card minion)
        {

        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {

        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {

        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {

        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {

        }

        public override void OnProcessAction(Action a, Board board)
        {
            float moveVal = board.TrapMgr.GetSecretModifier(a, board, true);
            GlobalValueModifier += (int)moveVal;
        }

        public override RemoteProfile DeepClone()
        {
            bProfile ret = new bProfile();
            ret.HeroEnemyHealthValue = HeroEnemyHealthValue;
            ret.HeroFriendHealthValue = HeroFriendHealthValue;
            ret.MinionEnemyAttackValue = MinionEnemyAttackValue;
            ret.MinionEnemyHealthValue = MinionEnemyHealthValue;
            ret.MinionFriendAttackValue = MinionFriendAttackValue;
            ret.MinionFriendHealthValue = MinionFriendHealthValue;

            ret.GlobalValueModifier = GlobalValueModifier;

            ret._logBestMove.AddRange(_logBestMove);
            ret._log = _log;

            return ret;
        }

        public bool CanSurvive(Card card, Board board)
        {
            if (card.MaxHealth > GetTotalEnemyDmg(board))
            {
                return true;
                //Debug("True");
            }
            else
            {
                return false;
                //Debug("False");
            }
        }

        public int GetTotalEnemyDmg(Board board)
        {
            int i = 0;

            foreach (Card card in board.MinionEnemy)
            {
                i = card.CurrentAtk + i;
            }

            i = board.HeroEnemy.CurrentAtk + i;

            return i;
        }

        private bool IsPaladinSecret(Board board)
        {
            //Check if our deck is Paladin secret
            return true;
        }

        private bool IsAggroOpponent(Board board)
        {
            if (board.TurnCount == 2 && board.EnemyGraveyard.Count + board.MinionEnemy.Count > 2 || board.TurnCount == 3 && board.EnemyGraveyard.Count + board.MinionEnemy.Count > 3 || board.TurnCount == 4 && board.EnemyGraveyard.Count + board.MinionEnemy.Count > 4 || board.TurnCount == 5 && board.EnemyGraveyard.Count + board.MinionEnemy.Count > 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsFreezeMageOpponent(Board board)
        {
            if (board.EnemyGraveyard.Count(x => x == Card.Cards.NEW1_021) >= 1 && board.HeroEnemy.Template.Id == Card.Cards.HERO_08)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsHandLockOpponent(Board board)
        {
            if (board.TurnCount == 3 && board.EnemyGraveyard.Count == 0 && board.HeroEnemy.Template.Id == Card.Cards.HERO_07)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}