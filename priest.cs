/*Smarbot Profile
* Deck from : http://www.hearthpwn.com/decks/242748-worgen-30-dmg-otk
* Contributors : Wbulot
*/

using System.Linq;
using System.Collections.Generic;

namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {

        private int MinionEnemyTauntValue = 5;
        private int MinionEnemyWindfuryValue = 6;
        private int MinionDivineShieldValue = 4;

        private int FriendCardDrawValue = 10;
        private int EnemyCardDrawValue = 5;

        private int HeroEnemyHealthValue = 1;
        private int HeroFriendHealthValue = 2;

        private int MinionEnemyAttackValue = 3;
        private int MinionEnemyHealthValue = 3;
        private int MinionFriendAttackValue = 1;
        private int MinionFriendHealthValue = 3;

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
            if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor >= 25)
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue - 1);
            }
            else if ((board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a) || board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor <= 15)
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue + 5);
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

            //Friend Card Draw Value
            if (board.FriendCardDraw + board.Hand.Count >= 10 || board.FriendDeckCount <= 5)
            {
                value -= 100;
            }
            else
            {
                if (board.Hand.Count >= 7)
                {
                    value += board.FriendCardDraw * 1;
                }
                else
                {
                    value += board.FriendCardDraw * FriendCardDrawValue;
                }
            }
            //Enemy Card Draw Value
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
                int MinionHearthCalc = card.CurrentHealth; //We limit minionhealth to 10, because it's often useless to have more
                if (MinionHearthCalc >= 10)
                {
                    MinionHearthCalc = 10;
                }
                value += MinionHearthCalc * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 5;

                if (card.IsDivineShield)
                    value += 5;
            }
            else
            {
                value += GetThreatModifier(card);

                if (card.Template.Id == Card.Cards.AT_049 && card.IsSilenced == false)
                {
                    value += 27;
                }

                if (card.Template.Id == Card.Cards.EX1_575 && card.IsSilenced == false)
                {
                    value += 5;
                }

                //Taunt value
                if (card.IsInspire && card.IsSilenced == false)
                    value += 5;

                if (card.IsTaunt)
                    value += MinionEnemyTauntValue;

                if (card.IsWindfury)
                    value += MinionEnemyWindfuryValue;

                if (card.IsDivineShield)
                    value += MinionDivineShieldValue;

                value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;
            }

            //Debug(value.ToString());
            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {

            switch (minion.Template.Id)
            {
                case Card.Cards.NEW1_020://Wild Pyromancer
                        MinionCastGlobalValue -= 14;
                        if (board.MinionFriend.Count == 0)
                        {
                            MinionCastGlobalValue += 7;
                        }
                        if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.NEW1_020).Count == 2)
                        {
                            MinionCastGlobalValue += 5;
                        }
                    break;

                case Card.Cards.GVG_014://Vol'jin
                    MinionCastGlobalValue -= 24;
                    break;

                case Card.Cards.EX1_591://Auchenai Soulpriest
                    MinionCastGlobalValue -= 4;
                    break;

                case Card.Cards.EX1_350://Prophet Velen
                    MinionCastGlobalValue -= 2;
                    break;

                case Card.Cards.CS2_235://Northshire Cleric
                    MinionCastGlobalValue -= 12;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_235).Count == 2)
                    {
                        MinionCastGlobalValue += 4;
                    }
                    if (board.TurnCount == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.BRM_017) >= 1)
                    {
                        MinionCastGlobalValue += 4;
                    }
                    break;

                case Card.Cards.EX1_091://Cabal Shadow Priest
                    MinionCastGlobalValue -= 6;
                    break;

                case Card.Cards.GVG_118://Troggzor the Earthinator
                    if (CanSurvive(minion, board))
                    {
                        MinionCastGlobalValue += 20;
                    }
                    break;

                case Card.Cards.GVG_110://Dr. Boom
                    MinionCastGlobalValue -= 5;
                    break;

                case Card.Cards.EX1_558://Harrison Jones
                    if (board.HeroEnemy.Template.Id == Card.Cards.HERO_01 || board.HeroEnemy.Template.Id == Card.Cards.HERO_01a || board.HeroEnemy.Template.Id == Card.Cards.HERO_04 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05 || board.HeroEnemy.Template.Id == Card.Cards.HERO_05a)
                    {
                        MinionCastGlobalValue -= 20;
                    }
                    if (board.WeaponEnemy != null)
                    {
                        MinionCastGlobalValue += 40;
                    }
                    break;

                case Card.Cards.GVG_069://Antique Healbot
                    if (board.HeroFriend.CurrentHealth > 23)
                    {
                        MinionCastGlobalValue -= 5;
                    }
                    MinionCastGlobalValue -= 15;
                    break;

                case Card.Cards.EX1_085://Mind Control Tech
                    if (board.MinionFriend.Count == 0)
                    {
                        MinionCastGlobalValue -= 5;
                    }
                    else
                    {
                        MinionCastGlobalValue -= 10;
                    }

                    if (board.MinionEnemy.Count >= 4)
                    {
                        MinionCastGlobalValue += 15;
                    }
                    break;

                case Card.Cards.FP1_001://Zombie Chow
                    if ((board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) < 15)
                    {
                        MinionCastGlobalValue -= 15;
                    }
                    else if ((board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) < 20)
                    {
                        MinionCastGlobalValue -= 10;
                    }
                    break;

                case Card.Cards.EX1_016://Sylvanas Windrunner
                        MinionCastGlobalValue += 5;
                    break;

                case Card.Cards.EX1_298://Ragnaros the Firelord
                    MinionCastGlobalValue -= 5;
                    break;

                case Card.Cards.FP1_009://Deathlord
                    if (board.TurnCount > 4 || board.MinionEnemy.Count == 0)
                    {
                        MinionCastGlobalValue -= 8;
                    }
                    MinionCastGlobalValue -= 5;
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
                if (board.Hand.FindAll(x => x.Race == Card.CRace.DEMON).Count == 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_310).Count == 1) //Some tweak here for higher chance to proc doomguard
                {
                    GlobalValueModifier += 5;
                }
            }

            if (minion.Template.Id == Card.Cards.EX1_029 && minion.IsFriend == false)//Leper Gnome
            {
                GlobalValueModifier += 6;
            }

            if (minion.Template.Id == Card.Cards.NEW1_019 && minion.IsFriend == false)//Knife Jugler
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

            if (minion.Template.Id == Card.Cards.FP1_007 && minion.IsFriend == true)//Nerubian Egg Friend
            {
                if (minion.IsTaunt)
                {
                    GlobalValueModifier -= 9;
                }
            }

            if (minion.Template.Id == Card.Cards.EX1_556 && minion.IsFriend == false)//Harvest Golem
            {
                GlobalValueModifier += 5;
            }

            if (minion.Template.Id == Card.Cards.NEW1_012 && minion.IsFriend == false && minion.IsSilenced == false)//Mana Wyrm
            {
                GlobalValueModifier += 5;
            }

            if (minion.Template.Id == Card.Cards.EX1_055 && minion.IsFriend == false && minion.IsSilenced == false)//Mana Addict
            {
                GlobalValueModifier += 5;
            }

            if (minion.Template.Id == Card.Cards.CS2_235 && minion.IsFriend == true && minion.IsSilenced == false)//Northshire Cleric
            {
                GlobalValueModifier -= 2;
            }

            if (minion.Template.Id == Card.Cards.CS2_235 && minion.IsFriend == false && minion.IsSilenced == false)//Northshire Cleric
            {
                GlobalValueModifier += 10;
            }

            if (minion.Template.Id == Card.Cards.AT_011 && minion.IsFriend == false && minion.IsSilenced == false)//Holy Champion
            {
                GlobalValueModifier += 7;
            }

            if (minion.Template.Id == Card.Cards.FP1_001 && minion.IsFriend == false && minion.IsSilenced == false)//Zombie Chow
            {
                if (board.HeroFriend.CurrentHealth >= 25)
                {
                    GlobalValueModifier -= 5;
                }
            }

            if (minion.Template.Id == Card.Cards.EX1_572 && minion.IsFriend == true && minion.IsSilenced == false)//Ysera
            {
                GlobalValueModifier -= 12;
            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {
                case Card.Cards.GVG_012://Light of the Naaru
                    SpellsCastGlobalValue -= 13;
                    break;

                case Card.Cards.AT_055://Flash Heal
                    SpellsCastGlobalValue -= 25;
                    //if (target.Type == Card.CType.MINION)
                    //{
                    //    SpellsCastGlobalValue += 10;
                    //}
                    break;

                case Card.Cards.CS2_004://Power Word: Shield
                    SpellsCastGlobalValue -= 10;
                    if (target.Template.Id == Card.Cards.EX1_007)
                    {
                        SpellsCastGlobalValue += 1;
                    }
                    break;

                case Card.Cards.GVG_010://Velen's Chosen
                    SpellsCastGlobalValue -= 14;
                    if (target.CanAttack)
                    {
                        SpellsCastGlobalValue += 7;
                    }
                    break;

                case Card.Cards.EX1_621://Circle of Healing
                    SpellsCastGlobalValue -= 17;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_621).Count == 2)
                    {
                        SpellsCastGlobalValue += 5;
                    }
                    if (board.FriendGraveyard.FindAll(x => x == Card.Cards.EX1_591).Count == 2)
                    {
                        SpellsCastGlobalValue += 5;
                    }
                    break;

                case Card.Cards.CS1_112://Holy Nova
                    if (board.MinionEnemy.Count == 1)
                    {
                        SpellsCastGlobalValue -= 15;
                    }
                    SpellsCastGlobalValue -= 17;
                    break;

                case Card.Cards.EX1_622://Shadow Word: Death
                    SpellsCastGlobalValue -= 13;
                    break;

                case Card.Cards.CS2_234://Shadow Word: Pain
                    SpellsCastGlobalValue -= 4;
                    break;

                case Card.Cards.GVG_008://Lightbomb
                    SpellsCastGlobalValue -= 24;
                    break;

                case Card.Cards.EX1_332://Silence
                    SpellsCastGlobalValue -= 14;
                    break;

                case Card.Cards.CS1_113://Mind Control
                    SpellsCastGlobalValue -= 27;
                    break;

                case Card.Cards.CS1_130://Holy Smite
                    SpellsCastGlobalValue -= 10;
                    break;

                case Card.Cards.BRM_017://Resurrect
                    SpellsCastGlobalValue += 15;
                    break;

                case Card.Cards.AT_016://Confuse
                    SpellsCastGlobalValue -= 3;
                    break;

                case Card.Cards.EX1_626://Mass Dispel
                    SpellsCastGlobalValue -= 10;
                    break;

                case Card.Cards.GAME_005://The Coin
                    SpellsCastGlobalValue -= 2;
                    break;
            }

            if (IsSparePart(spell))
            {
                SpellsCastGlobalValue += 2;
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
                    GlobalValueModifier += 5;
                }
            }

            if (board.HeroEnemy.CurrentHealth >= 27 && attacker.Template.Id == Card.Cards.FP1_001 && target.Type == Card.CType.HERO) //Avoid zombie show attack face if enemy is high health
            {
                GlobalValueModifier -= 5;
            }
        }
        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            HeroPowerGlobalCost += 1;

            if (board.TurnCount == 1) //Avoid coin Heropower
                HeroPowerGlobalCost += 7;
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

            ret._logBestMove.AddRange(_logBestMove);
            ret._log = _log;

            ret.SecretModifier = SecretModifier;

            return ret;
        }

        public float GetThreatModifier(Card card)
        {
            switch (card.Template.Id)
            {
                case Card.Cards.GVG_006://Mechwarper
                    return 4;

                case Card.Cards.EX1_412://Raging Worgen
                    return 4;

                case Card.Cards.FP1_005://Shade of Naxxramas
                    return 4;

                case Card.Cards.FP1_013://Kel'Thuzad
                    return 6;

                case Card.Cards.EX1_016://Sylvanas Windrunner
                    return 5;

                case Card.Cards.GVG_105://Piloted Sky Golem
                    return 3;

                case Card.Cards.BRM_031://Chromaggus
                    return 5;

                case Card.Cards.EX1_559://Archmage Antonidas
                    return 8;

                case Card.Cards.GVG_021://Mal'Ganis
                    return 6;

                case Card.Cards.EX1_608://Sorcerer's Apprentice
                    return 3;

                case Card.Cards.NEW1_012://Mana Wyrm
                    return 3;

                case Card.Cards.BRM_002://Flamewaker
                    return 4;

                case Card.Cards.EX1_595://Cult Master
                    return 2;

                case Card.Cards.NEW1_021://Doomsayer
                    return 0;

                case Card.Cards.EX1_243://Dust Devil
                    return 2;

                case Card.Cards.EX1_170://Emperor Cobra
                    return 4;

                case Card.Cards.BRM_028://Emperor Thaurissan
                    return 6;

                case Card.Cards.EX1_565://Flametongue Totem
                    return 5;

                case Card.Cards.GVG_100://Floating Watcher
                    return 4;

                case Card.Cards.GVG_113://Foe Reaper 4000
                    return 0;

                case Card.Cards.tt_004://Flesheating Ghoul
                    return 2;

                case Card.Cards.EX1_604://Frothing Berserker
                    return 3;

                case Card.Cards.BRM_019://Grim Patron
                    return 7;

                case Card.Cards.EX1_084://Warsong Commander
                    return 7;

                case Card.Cards.EX1_095://Gadgetzan Auctioneer
                    return 4;

                case Card.Cards.NEW1_040://Hogger
                    return 3;

                case Card.Cards.GVG_104://Hobgoblin
                    return 5;

                case Card.Cards.EX1_614://Illidan Stormrage
                    return 4;

                case Card.Cards.GVG_027://Iron Sensei
                    return 5;

                case Card.Cards.GVG_094://Jeeves
                    return 5;

                case Card.Cards.NEW1_019://Knife Juggler
                    return 4;

                case Card.Cards.EX1_001://Lightwarden
                    return 4;

                case Card.Cards.EX1_563://Malygos
                    return 6;

                case Card.Cards.GVG_103://Micro Machine
                    return 3;

                case Card.Cards.EX1_044://Questing Adventurer
                    return 4;

                case Card.Cards.EX1_298://Ragnaros the Firelord
                    return 6;

                case Card.Cards.GVG_037://Whirling Zap-o-matic
                    return 4;

                case Card.Cards.GVG_013://Cogmaster
                    return 5;
            }

            return 0;
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

                default:
                    return false;
            }
            return false;
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
    }
}