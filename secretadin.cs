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
        private int MinionEnemyWindfuryValue = 4;
        private int MinionDivineShield = 2;

        private int FriendCardDrawValue = 6;
        private int EnemyCardDrawValue = 0;

        private int HeroEnemyHealthValue = 3;
        private int HeroFriendHealthValue = 1;

        private int MinionEnemyAttackValue = 3;
        private int MinionEnemyHealthValue = 2;
        private int MinionFriendAttackValue = 3;
        private int MinionFriendHealthValue = 2;

        //Spells cast value
        private int SpellsCastGlobalValue = 0;

        //Weapons cast value
        private int WeaponCastGlobalValue = 0;

        //Minions cast value
        private int MinionCastGlobalValue = 0;

        //HeroPowerCost
        private int HeroPowerGlobalCost = 0;

        //Weapons Attack cost
        private int WeaponAttackGlobalCost = 0;

        //GlobalValueModifier
        private int GlobalValueModifier = 0;

        //Secret Modifier
        private int SecretModifier = 0;

        public override float GetBoardValue(Board board)
        {
            float value = 0;

            //Hero friend value
            if (board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a) //Against hunter, save more health
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue + 2);
            }
            else
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;
            }

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

            //casting action value
            value += WeaponCastGlobalValue;
            value += SpellsCastGlobalValue;
            value += MinionCastGlobalValue;

            //heropower cost
            value -= HeroPowerGlobalCost;

            //Weapon attack cost
            value -= WeaponAttackGlobalCost;

            //Lethal and save my ass
            if (board.HeroEnemy.CurrentHealth <= 0)
                value += 100000;

            if (board.HeroFriend.CurrentHealth <= 0)
                value -= 100000;

            value += GlobalValueModifier;

            value += board.FriendCardDraw * FriendCardDrawValue;
            value -= board.EnemyCardDraw * EnemyCardDrawValue;

            value += SecretModifier;

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

                if (card.IsDivineShield)
                    value += MinionDivineShield * (card.CurrentAtk / 2);
            }
            else
            {
                if (board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a)//If hunter opponent, little more control
                {
                    value += card.CurrentHealth * (MinionEnemyHealthValue) + card.CurrentAtk * (MinionEnemyAttackValue + 2);
                }
                else
                {
                    if (board.TurnCount <= 4)//If before turn 4, little more control
                    {
                        value += card.CurrentHealth * (MinionEnemyHealthValue) + card.CurrentAtk * (MinionEnemyAttackValue + 1);
                    }
                    else
                    {
                        value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;
                    }
                }

                //Taunt value
                if (card.IsTaunt)
                    value += MinionEnemyTauntValue;

                //Windfury value
                if (card.IsWindfury)
                    value += MinionEnemyWindfuryValue;

                //divine shield value
                if (card.IsDivineShield)
                    value += MinionDivineShield;

                //Dangerous minion value
                if (card.Template.Id == Card.Cards.EX1_412 && card.IsSilenced == false && card.IsFriend == false) //Raging Worgen
                {
                    value += 10;
                }

                if (card.Template.Id == Card.Cards.EX1_565 && card.IsSilenced == false && card.IsFriend == false) //Flametongue Totem
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.BRM_002 && card.IsSilenced == false && card.IsFriend == false) //Flamewaker
                {
                    value += 10;
                }

                if (card.Template.Id == Card.Cards.EX1_402 && card.IsSilenced == false && card.IsFriend == false) //Armorsmith
                {
                    value += 10;
                }

                if (card.Template.Id == Card.Cards.GVG_006 && card.IsSilenced == false && card.IsFriend == false) //Mechwarper
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.FP1_005 && card.IsSilenced == false && card.IsFriend == false) //Shade of Naxxramas
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.FP1_013 && card.IsSilenced == false && card.IsFriend == false) //Kel'Thuzad
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.BRM_031 && card.IsSilenced == false && card.IsFriend == false) //Chromaggus
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_559 && card.IsSilenced == false && card.IsFriend == false) //Archmage Antonidas
                {
                    value += 10;
                }

                if (card.Template.Id == Card.Cards.GVG_021 && card.IsSilenced == false && card.IsFriend == false) //Mal'Ganis
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_608 && card.IsSilenced == false && card.IsFriend == false) //Sorcerer's Apprentice
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.NEW1_012 && card.IsSilenced == false && card.IsFriend == false) //Mana Wyrm
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_595 && card.IsSilenced == false && card.IsFriend == false) //Cult Master
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_170 && card.IsSilenced == false && card.IsFriend == false) //Emperor Cobra
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.BRM_028 && card.IsSilenced == false && card.IsFriend == false) //Emperor Thaurissan
                {
                    value += 15;
                }

                if (card.Template.Id == Card.Cards.GVG_100 && card.IsSilenced == false && card.IsFriend == false) //Floating Watcher
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.tt_004 && card.IsSilenced == false && card.IsFriend == false) //Flesheating Ghoul
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_604 && card.IsSilenced == false && card.IsFriend == false) //Frothing Berserker
                {
                    value += 20;
                }

                if (card.Template.Id == Card.Cards.BRM_019 && card.IsSilenced == false && card.IsFriend == false) //Grim Patron
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_084 && card.IsSilenced == false && card.IsFriend == false) //Warsong Commander
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_095 && card.IsSilenced == false && card.IsFriend == false) //Gadgetzan Auctioneer
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.NEW1_040 && card.IsSilenced == false && card.IsFriend == false) //Hogger
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.GVG_104 && card.IsSilenced == false && card.IsFriend == false) //Hobgoblin
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_614 && card.IsSilenced == false && card.IsFriend == false) //Illidan Stormrage
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.GVG_027 && card.IsSilenced == false && card.IsFriend == false) //Iron Sensei
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.GVG_094 && card.IsSilenced == false && card.IsFriend == false) //Jeeves
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.NEW1_019 && card.IsSilenced == false && card.IsFriend == false) //Knife Juggler
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_001 && card.IsSilenced == false && card.IsFriend == false) //Lightwarden
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_563 && card.IsSilenced == false && card.IsFriend == false) //Malygos
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.GVG_103 && card.IsSilenced == false && card.IsFriend == false) //Micro Machine
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_044 && card.IsSilenced == false && card.IsFriend == false) //Questing Adventurer
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.NEW1_020 && card.IsSilenced == false && card.IsFriend == false) //Wild Pyromancer
                {
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.GVG_013 && card.IsSilenced == false && card.IsFriend == false) //Cogmaster
                {
                    value += 5;
                }
            }

            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.CS2_188://Abusive Sergeant
                    //if (target != null)
                    //{
                    //    if (target.CanAttack == false || target.IsFriend == false)
                    //    {
                    //        MinionCastGlobalValue -= 6;
                    //    }
                    //}
                    MinionCastGlobalValue -= 9;
                    if (board.MinionEnemy.Count == 0) //Avoid play abusive if empty enemy board (use it for trade)
                    {
                        MinionCastGlobalValue -= 4;
                    }
                    break;

                case Card.Cards.AT_079://Mysterious Challenger
                    MinionCastGlobalValue += 10;
                    break;

                case Card.Cards.FP1_002://Haunted Creeper
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.NEW1_019) >= 1 && board.ManaAvailable <= 3)
                    {
                        MinionCastGlobalValue += 7;
                    }
                    break;

                case Card.Cards.CS2_203://Ironbeak Owl
                    MinionCastGlobalValue -= 15;
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_203) == 2)
                    {
                        MinionCastGlobalValue += 10;
                    }

                    if (board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a)//Inscrease value of owl against hunter
                    {
                        if (target != null && target.IsFriend == false)
                        {
                            MinionCastGlobalValue += 10;
                        }

                        if (target != null && target.Template.Id == Card.Cards.FP1_004) //Inscrease value of owl against mad scientitst
                        {
                            MinionCastGlobalValue += 3;
                        }
                    }

                    if (target != null && target.Template.Id == Card.Cards.NEW1_021 && target.IsSilenced == false) //Inscrease value of owl against doomsayer
                    {
                        MinionCastGlobalValue += 110;
                    }

                    break;

                case Card.Cards.AT_076://Murloc Knight
                        MinionCastGlobalValue -= 5;
                    break;

                case Card.Cards.NEW1_019://Knife Juggler
                    if (CanSurvive(minion, board) == false)
                    {
                        MinionCastGlobalValue -= 2;
                    }
                    break;
            }

            foreach (Card card in board.MinionEnemy)
            {
                if (card.Template.Id == Card.Cards.NEW1_021 && card.IsSilenced == false)//Tweak Doomsayer
                {
                    MinionCastGlobalValue -= 100;
                }
            }
        }

        public override void OnMinionDeath(Board board, Card minion)
        {
            if (minion.Template.Id == Card.Cards.FP1_004 && minion.IsFriend == false && minion.IsSilenced == false)//Mad Scientist
            {
                GlobalValueModifier -= 20;
            }

            if (minion.Template.Id == Card.Cards.FP1_022 && minion.IsFriend == false && minion.IsSilenced == false)//Voidcaller
            {
                GlobalValueModifier -= 5;
            }

            if (minion.Template.Id == Card.Cards.FP1_022 && minion.IsFriend == true && minion.IsSilenced == false)//Voidcaller
            {
                GlobalValueModifier -= 5;
                if (board.Hand.Count(x => x.Race == Card.CRace.DEMON) == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 1) //Some tweak here for higher chance to proc doomguard
                {
                    GlobalValueModifier += 5;
                }
            }

            if (minion.Template.Id == Card.Cards.EX1_029 && minion.IsFriend == false)//Leper Gnome
            {
                GlobalValueModifier += 6;
            }

            if (minion.Template.Id == Card.Cards.FP1_002 && minion.IsFriend == true)//Haunted Creeper Friend
            {
                GlobalValueModifier -= 5;
            }

            if (minion.Template.Id == Card.Cards.GVG_096 && minion.IsFriend == false)//Piloted Shredder
            {
                GlobalValueModifier += 5;
            }

            if (minion.Template.Id == Card.Cards.FP1_007 && minion.IsFriend == true && minion.IsTaunt)//Nerubian Egg Friend
            {
                GlobalValueModifier -= 9;
            }

            if (minion.Template.Id == Card.Cards.EX1_556 && minion.IsFriend == false)//Harvest Golem
            {
                GlobalValueModifier += 5;
            }

            if (minion.Template.Id == Card.Cards.EX1_029 && minion.IsFriend == true && minion.IsSilenced == false)//Leper Gnome
            {
                GlobalValueModifier -= 5;
            }

            if (minion.Template.Id == Card.Cards.AT_123 && minion.IsFriend == false && minion.IsSilenced == false)//Chillmaw
            {
                GlobalValueModifier += 13;
            }

            if (minion.Template.Id == Card.Cards.NEW1_021 && minion.IsSilenced == false && minion.IsFriend == false)//Doomsayer
            {
                GlobalValueModifier += 1000;
            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {
                case Card.Cards.EX1_349://Divine Favor
                    if ((board.Hand.Count > board.EnemyCardCount))
                    {
                        SpellsCastGlobalValue -= 6;
                    }
                    break;

                case Card.Cards.AT_073://Competitive Spirit
                    if (board.MinionFriend.Count <= 1)
                    {
                        SpellsCastGlobalValue -= 5;
                    }
                    else
                    {
                        SpellsCastGlobalValue += 2;
                    }
                    break;

                case Card.Cards.CS2_093://Consecration
                        SpellsCastGlobalValue -= 11;
                    break;

                case Card.Cards.CS2_092://Blessing of Kings
                    if (target.IsDivineShield && target.IsFriend == true)
                    {
                        SpellsCastGlobalValue += 1;
                    }
                    break;

                case Card.Cards.EX1_379://Repentance
                    if (board.TurnCount == 1)
                    {
                        SpellsCastGlobalValue -= 5;
                    }
                    break;

                case Card.Cards.EX1_136://Redemption
                //Check if there is important minion on board
                        SpellsCastGlobalValue -= 1;
                    break;

                case Card.Cards.EX1_130://Noble Sacrifice
                    if (board.MinionFriend.Count == 0)
                    {
                        SpellsCastGlobalValue -= 3;
                    }
                    break;

                case Card.Cards.GVG_061://Muster for Battle
                        SpellsCastGlobalValue -= 2;
                        if (board.WeaponEnemy != null && board.WeaponEnemy.CurrentDurability <= 1 && board.WeaponEnemy.Template.Id == Card.Cards.FP1_021)
                        {
                            SpellsCastGlobalValue -= 8;
                        }
                        if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 && board.WeaponFriend.Template.Id == Card.Cards.CS2_097)
                        {
                            SpellsCastGlobalValue -= 5;
                        }
                        if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability > 1 && board.WeaponFriend.Template.Id == Card.Cards.EX1_383t)
                        {
                            SpellsCastGlobalValue -= 13;
                        }
                    break;

                case Card.Cards.GAME_005://The Coin
                    SpellsCastGlobalValue -= 4;
                    break;
            }

            if (spell.Template.IsSecret)
            {
                SpellsCastGlobalValue += 5;
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
            //if (weapon.Template.Id == Card.Cards.CS2_097)
            //{
            //    if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1)
            //    {
            //        SpellsCastGlobalValue -= 5;
            //    }
            //}
            if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 && board.WeaponFriend.Template.Id != Card.Cards.CS2_091)
            {
                GlobalValueModifier -= 6;
            }

            //if (board.WeaponFriend != null && board.WeaponFriend.Template.Id == Card.Cards.GVG_059)
            //{
               
            //}
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {

        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            HeroPowerGlobalCost += 0;
            if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.AT_076) >= 1)
            {
                HeroPowerGlobalCost -= 5;
            }
        }

        public override void OnProcessAction(Action a, Board board)
        {
            float moveVal = board.TrapMgr.GetSecretModifier(a, board, true);
            //Debug(moveVal.ToString());
            if (board.EnemyClass != Card.CClass.MAGE)
            {
                SecretModifier += (int)moveVal;
            }
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

            ret.SpellsCastGlobalValue = SpellsCastGlobalValue;
            ret.WeaponCastGlobalValue = WeaponCastGlobalValue;
            ret.MinionCastGlobalValue = MinionCastGlobalValue;

            ret.HeroPowerGlobalCost = HeroPowerGlobalCost;
            ret.WeaponAttackGlobalCost = WeaponAttackGlobalCost;

            ret.GlobalValueModifier = GlobalValueModifier;

            ret.SecretModifier = SecretModifier;

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
                i += card.CurrentAtk;
            }
            i += board.HeroEnemy.CurrentAtk;

            return i;
        }

        public static int Get2HpMinions(Board b)
        {
            int i = 0;

            foreach (Card card in b.MinionFriend)
            {
                if (card.CurrentHealth == 2 && card.IsDivineShield == false)
                    i++;
            }

            return i;
        }

        public static int GetWeakMinions(Board b)
        {
            int i = 0;

            foreach (Card card in b.MinionFriend)
            {
                if (card.CurrentHealth <= 2 && card.IsDivineShield == false)
                    i++;
            }

            return i;
        }

        public static int GetCanAttackMinions(Board b)
        {
            int i = 0;

            foreach (Card card in b.MinionFriend)
            {
                if (card.CanAttack)
                    i++;
            }
            return i;
        }
    }
}