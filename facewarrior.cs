//Smarbot Profile
//Deck from :
//Contributors : Wbulot

using System.Linq;
using System.Collections.Generic;

namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {

        private int MinionEnemyTauntValue = 10;
        private int MinionEnemyWindfuryValue = 4;
        private int MinionDivineShield = 2;
        private int MinionStealthValue = 2;
        private int MinionNotTargetableValue = 3;

        private int FriendCardDrawValue = 6;
        private int EnemyCardDrawValue = 5;

        private int HeroEnemyHealthValue = 4;
        private int HeroFriendHealthValue = 2;

        private int MinionEnemyAttackValue = 1;
        private int MinionEnemyHealthValue = 1;
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
            if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor > 20)
            {
                
            }
            else
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;
            }

            //Hero enemy value
            value -= (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) * HeroEnemyHealthValue;

            ////Weapon equiped value
            //if (board.WeaponFriend != null && board.HeroFriend.CanAttack == false)
            //{
            //    int weaponValue = board.WeaponFriend.CurrentAtk * board.WeaponFriend.CurrentDurability;
            //    value += weaponValue;
            //}

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
                    value += MinionDivineShield * (card.CurrentAtk / 2) + 1;

                if (card.IsStealth)
                    value += MinionStealthValue * (card.CurrentAtk / 2) + 1;

                if (card.IsTargetable == false)
                    value += MinionNotTargetableValue;
            }
            else
            {
                value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;

                //Taunt value
                if (card.IsTaunt)
                    value += MinionEnemyTauntValue;

                //Windfury value
                if (card.IsWindfury)
                    value += MinionEnemyWindfuryValue;

                //divine shield value
                if (card.IsDivineShield)
                    value += MinionDivineShield * (card.CurrentAtk / 2) + 1;

                //notTargetable value
                if (card.IsTargetable == false)
                    value += MinionNotTargetableValue;

                if (card.IsFrozen)
                    value += MinionStealthValue * (card.CurrentAtk / 2) + 1;

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
                    value += 5;
                }

                if (card.Template.Id == Card.Cards.EX1_402 && card.IsSilenced == false && card.IsFriend == false) //Armorsmith
                {
                    value += 15;
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
                    value += 10;
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

                if (card.Template.Id == Card.Cards.CS2_237 && card.IsSilenced == false && card.IsFriend == false) //Starving Buzzard
                {
                    value += 10;
                }

                if (card.Template.Id == Card.Cards.EX1_080 && card.IsSilenced == false && card.IsFriend == false) //Secretkeeper
                {
                    value += 7;
                }

                if (card.Template.Id == Card.Cards.GVG_002 && card.IsSilenced == false && card.IsFriend == false) //Snowchugger
                {
                    value += 10;
                }
            }

            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.GVG_013://Cogmaster
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.GVG_051) >= 1)
                    {
                        MinionCastGlobalValue += 3;
                    }
                    break;

                case Card.Cards.GVG_102://Tinkertown Technician
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                    {
                        MinionCastGlobalValue -= 4;
                    }
                    break;

                case Card.Cards.GVG_055://Screwjank Clunker
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                    {
                        MinionCastGlobalValue -= 5;
                    }
                    break;

                case Card.Cards.CS2_188://Abusive Sergeant
                        MinionCastGlobalValue -= 7;
                    break;

                case Card.Cards.EX1_089://Arcane Golem
                    MinionCastGlobalValue -= 12;
                    break;

                case Card.Cards.CS2_146://Southsea Deckhand
                    if (board.WeaponFriend == null)
                    {
                        MinionCastGlobalValue -= 10;
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
                        if (target != null && target.Template.Id == Card.Cards.FP1_004) //Inscrease value of owl against mad scientitst
                        {
                            MinionCastGlobalValue += 5;
                        }
                    }

                    if (target != null && target.Template.Id == Card.Cards.NEW1_021 && target.IsSilenced == false) //Inscrease value of owl against doomsayer
                    {
                        MinionCastGlobalValue += 110;
                    }

                    if (target != null && target.IsTaunt == true) //Inscrease value of owl against taunt
                    {
                        MinionCastGlobalValue += 10;
                    }

                    break;
            }

            if (minion.IsCharge)
            {
                MinionCastGlobalValue -= 10;
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

            if (minion.Template.Id == Card.Cards.FP1_007 && minion.IsFriend == true && minion.IsTaunt)//Nerubian Egg Friend
            {
                GlobalValueModifier -= 9;
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
                case Card.Cards.CS2_105://Heroic Strike
                        SpellsCastGlobalValue -= 16;
                    break;

                case Card.Cards.EX1_408://Mortal Strike
                    SpellsCastGlobalValue -= 16;
                    if (target != board.HeroEnemy || target.IsTaunt == false)
                    {
                        SpellsCastGlobalValue -= 5;
                    }
                    break;

                case Card.Cards.AT_064://Bash
                    SpellsCastGlobalValue -= 12;
                    if (target.IsTaunt)
                    {
                        SpellsCastGlobalValue += 4;
                    }
                    break;

                case Card.Cards.GAME_005://The Coin
                    SpellsCastGlobalValue -= 4;
                    break;
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
            GlobalValueModifier += 1;
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {
            if (board.WeaponFriend != null && attacker.Type == Card.CType.WEAPON)
            {
                if (target == board.HeroEnemy && board.MinionEnemy.Count > 0 && board.TurnCount <= 3)
                {
                    GlobalValueModifier -= 5;
                }

                if (board.WeaponFriend.Template.Id == Card.Cards.FP1_021 && board.WeaponFriend.CurrentDurability == 1 && target.CurrentHealth == 1 && !target.IsTaunt) //If Death's Bite durability is 1, avoid to directly attack 1 health minion (unless if its taunt)
                {
                    WeaponAttackGlobalCost += 46;
                }
            }

            if (target.Template.Id == Card.Cards.EX1_007 && target.IsFriend == false) //Le but est de minimiser la pioche de l'adversaire, en tuant d'un coup l'acolyte
            {
                if (attacker.CurrentAtk >= target.CurrentHealth)
                {
                    //Debug("On peut one shot l'acolyte : attacker ATK : " + attacker.CurrentAtk + " target Heath : " + target.CurrentHealth);
                    GlobalValueModifier += 4;
                    if (board.HeroEnemy.Template.Id == Card.Cards.HERO_01 || board.HeroEnemy.Template.Id == Card.Cards.HERO_01a)
                    {
                        GlobalValueModifier += 4;
                    }
                }
            }

            //if (GetTotalDmg(board) + GetDmgInHand(board) >= (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor)) //Go face for lethal next turn !
            //{
            //    if (target == board.HeroEnemy)
            //    {
            //        Debug("lethal next turn, gogogo");
            //        GlobalValueModifier += 50;
            //    }
            //}
        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            HeroPowerGlobalCost -= 1;
        }

        public override void OnProcessAction(Action a, Board board)
        {
            float moveVal = board.TrapMgr.GetSecretModifier(a, board, true);
            //Debug(moveVal.ToString());
            SecretModifier += (int)moveVal;
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

        public int GetTotalDmg(Board board)
        {
            int i = 0;
            foreach (Card card in board.MinionFriend)
            {
                if (card.CanAttack)
                {
                    i += card.CurrentAtk;
                }
            }
            if (board.HeroFriend.CanAttack)
            {
                i += board.HeroFriend.CurrentAtk;
            }

            return i;
        }

        public int GetDmgInHand(Board board)
        {
            int iw = 0;
            int i = 0;

            foreach (Card card in board.Hand)
            {
                if (card.Type == Card.CType.WEAPON)//Weapons
                {
                    if (card.CurrentAtk >= iw)
                    {
                        iw = card.CurrentAtk;
                    }
                }
                if (card.Template.Id == Card.Cards.NEW1_011)//Kor'kron Elite
                {
                    i += 4;
                }
                if (card.Template.Id == Card.Cards.CS2_105) //Heroic strike
                {
                    i += 4;
                }
                if (card.Template.Id == Card.Cards.EX1_408) //Mortal Strike
                {
                    if (board.HeroFriend.CurrentHealth < 12)
                    {
                        i += 6;
                    }
                    else
                    {
                        i += 4;
                    }
                }
            }

            i += iw; //We add weapon dmg
            return i;
        }

        public static bool IsSparePart(Card c)
        {
            switch (c.Template.Id)
            {
                case Card.Cards.PART_001:
                    return true;
                case Card.Cards.PART_002:
                    return true;
                case Card.Cards.PART_003:
                    return true;
                case Card.Cards.PART_004:
                    return true;
                case Card.Cards.PART_005:
                    return true;
                case Card.Cards.PART_006:
                    return true;
                case Card.Cards.PART_007:
                    return true;
            }
            return false;
        }
    }
}