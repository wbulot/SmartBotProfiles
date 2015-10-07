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
        private int MinionEnemyWindfuryValue = 6;
        private int MinionDivineShield = 4;

        private int FriendCardDrawValue = 2;
        private int EnemyCardDrawValue = 3;

        private int HeroEnemyHealthValue = 2;
        private int HeroFriendHealthValue = 2;

        private int MinionEnemyAttackValue = 3;
        private int MinionEnemyHealthValue = 2;
        private int MinionFriendAttackValue = 2;
        private int MinionFriendHealthValue = 2;

        //Spells cast cost
        private int SpellsCastGlobalCost = 0;
        //Spells cast value
        private int SpellsCastGlobalValue = 0;

        //Weapons cast cost
        private int WeaponCastGlobalCost = 0;
        //Weapons cast value
        private int WeaponCastGlobalValue = 0;

        //Minions cast cost
        private int MinionCastGlobalCost = 0;
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
            if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor > 12)
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;
            }
            else //If life bellow 12, critical zone, we try to save our health
            {
                if (board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a) //Against hunter, save more health
                {
                    value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue + 27);
                }
                else
                {
                    value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue + 5);
                }
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

            //casting costs
            value -= MinionCastGlobalCost;
            value -= SpellsCastGlobalCost;
            value -= WeaponCastGlobalCost;

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

            if (board.HeroFriend.CurrentHealth <= 0 && board.FriendCardDraw == 0)
                value -= 100000;

            value += GlobalValueModifier;

            value += board.FriendCardDraw * FriendCardDrawValue;
            value -= board.EnemyCardDraw * EnemyCardDrawValue;

            return value;
        }

        public float GetCardValue(Board board, Card card)
        {
            float value = 0;

            if (card.IsFriend)
            {
                value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 5;

                if (card.Template.Id == Card.Cards.FP1_007 && card.IsTaunt)//Nerubian Egg
                {
                    value += 5;
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
                    value += 7;
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

            //Debug(value.ToString());
            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.EX1_319://Flame Imp
                    if (board.TurnCount == 1)
                    {
                        MinionCastGlobalValue += 5;
                    }
                    break;

                case Card.Cards.EX1_310://Doomguard
                    MinionCastGlobalValue -= 35;
                    if (board.Hand.Count <= 2)
                    {
                        MinionCastGlobalValue += 9;
                        if (board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_316) == 1 && board.ManaAvailable >= 6)
                        {
                            MinionCastGlobalValue -= 30;
                        }
                    }

                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 2)
                    {
                        SpellsCastGlobalValue += 8;
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

                case Card.Cards.FP1_007://Nerubian Egg
                    MinionCastGlobalValue += 6;
                    break;

                case Card.Cards.EX1_162://Dire Wolf Alpha
                    if (board.MinionFriend.Count == 0)
                    {
                        MinionCastGlobalValue -= 2;
                    }
                    break;

                case Card.Cards.FP1_002://Haunted Creeper
                    MinionCastGlobalValue += 5;
                    break;

                case Card.Cards.CS2_188://Abusive Sergeant
                    //if (target != null)
                    //{
                    //    if (target.CanAttack == false || target.IsFriend == false)
                    //    {
                    //        MinionCastGlobalValue -= 6;
                    //    }
                    //}
                    MinionCastGlobalValue -= 7;
                    if (board.MinionEnemy.Count == 0) //Avoid play abusive if empty enemy board (use it for trade)
                    {
                        MinionCastGlobalValue -= 4;
                    }

                    break;

                case Card.Cards.EX1_304://Void Terror
                    MinionCastGlobalValue -= 10;
                    if (board.MinionFriend.Count == 0 && board.TurnCount > 3)
                    {
                        MinionCastGlobalValue -= 5;
                    }

                    break;

                case Card.Cards.CS2_064://Dread Infernal
                    MinionCastGlobalValue -= 4;
                    break;

                case Card.Cards.FP1_022://Voidcaller
                    if (board.Hand.Count(x => x.Race == Card.CRace.DEMON) > 1)
                    {
                        MinionCastGlobalValue += 5;
                    }

                    break;

                case Card.Cards.AT_122://Gormok the Impaler
                    if (board.MinionFriend.Count < 4)
                    {
                        MinionCastGlobalValue -= 5;
                    }
                    break;

                case Card.Cards.NEW1_019://Knife Juggler
                    MinionCastGlobalValue += 0;
                    if (CanSurvive(minion, board) == false || board.HeroEnemy.Template.Id == Card.Cards.HERO_03)
                    {
                        MinionCastGlobalValue -= 2;
                    }

                    break;

                case Card.Cards.EX1_093://Defender of Argus
                    MinionCastGlobalValue -= 3;
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

            if (minion.Template.Id == Card.Cards.EX1_402 && minion.IsFriend == false)//Armorsmith
            {
                GlobalValueModifier += 3;
            }

            if (minion.Template.Id == Card.Cards.FP1_002 && minion.IsFriend == true)//Haunted Creeper Friend
            {
                GlobalValueModifier -= 5;
            }

            if (minion.Template.Id == Card.Cards.NEW1_019 && minion.IsFriend == true)//Knife Jugler Friend
            {
                GlobalValueModifier -= 7;
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

            if (minion.Template.Id == Card.Cards.NEW1_021 && minion.IsSilenced == false && minion.IsFriend == false)//Doomsayer
            {
                GlobalValueModifier += 1000;
            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {
                case Card.Cards.EX1_316://Power Overwhelming
                    SpellsCastGlobalValue -= 9;
                    if (target.CanAttack == false)
                    {
                        SpellsCastGlobalValue -= 18;
                    }

                    if (board.Hand.Count <= 2)
                    {
                        bool hasDoomGuardInHand = (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.FP1_007) >= 1 && board.ManaAvailable >= 6);
                        foreach (Card card in board.Hand)
                        {
                            if (card.Template.Id == Card.Cards.EX1_310 && hasDoomGuardInHand)//We have doomgard and PO in hand, egg on board, use PO before doomguard
                            {
                                SpellsCastGlobalValue += 15;
                            }
                        }
                    }

                    if (board.MinionEnemy.Count == 0) //Avoid play PO if empty enemy board (use it for trade)
                    {
                        SpellsCastGlobalValue -= 3;
                    }

                    break;

                case Card.Cards.EX1_320://Bane of Doom
                    if (target.CurrentHealth <= 2 && board.MinionFriend.Count < 7)
                    {
                        SpellsCastGlobalValue += 10;
                    }
                    else
                    {
                        SpellsCastGlobalValue -= 10;
                    }

                    break;

                case Card.Cards.GVG_045://Imp-losion
                    SpellsCastGlobalValue -= 13;
                    if (target.IsFriend)
                    {
                        SpellsCastGlobalValue -= 6;
                    }

                    if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.NEW1_019) >= 1)
                    {
                        //Debug("combo knife implosion");
                        SpellsCastGlobalValue += 10;
                    }

                    if (board.WeaponEnemy != null && board.WeaponEnemy.CurrentDurability <= 1 && board.WeaponEnemy.Template.Id == Card.Cards.FP1_021)
                    {
                        SpellsCastGlobalValue -= 8;
                    }

                    break;

                case Card.Cards.BRM_005://Demonwrath
                    SpellsCastGlobalValue -= 5;
                    break;

                case Card.Cards.EX1_302://Mortal Coil
                    SpellsCastGlobalValue -= 5;
                    if (target.IsFriend)
                    {
                        SpellsCastGlobalValue -= 7;
                    }

                    if (target.CurrentHealth == 1)
                    {
                        SpellsCastGlobalValue += 4;
                    }

                    break;

                case Card.Cards.EX1_596://Demonfire
                    SpellsCastGlobalValue -= 9;
                    if (target.IsFriend && target.CanAttack)
                    {
                        SpellsCastGlobalValue += 4;
                    }

                    break;

                case Card.Cards.GVG_015://Darkbomb
                    SpellsCastGlobalValue -= 5;
                    break;

                case Card.Cards.GAME_005://The Coin
                    SpellsCastGlobalValue -= 4;
                    break;
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {
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
        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            HeroPowerGlobalCost -= 3;
            //if (board.TurnCount == 1) //Avoid coin Heropower
            //    HeroPowerGlobalCost += 7;

            if (board.Hand.Count == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 1) //Avoid HeroPower if Doomguard in hand and alone
            {
                HeroPowerGlobalCost += 3;
            }

            if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.FP1_022) >= 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) >= 1) //Avoid HeroPower if we have Voidcaller on board and Doomguard in Hand
            {
                HeroPowerGlobalCost += 3;
            }
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

            ret.SpellsCastGlobalCost = SpellsCastGlobalCost;
            ret.SpellsCastGlobalValue = SpellsCastGlobalValue;
            ret.WeaponCastGlobalCost = WeaponCastGlobalCost;
            ret.WeaponCastGlobalValue = WeaponCastGlobalValue;
            ret.MinionCastGlobalCost = MinionCastGlobalCost;
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
                i = card.CurrentAtk + i;
            }

            i = board.HeroEnemy.CurrentAtk + i;

            return i;
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