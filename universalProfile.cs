//Smarbot Universal Profile
//Contributors : Wbulot
//Decks supported : Paladin Secret - Zoo - Worgen OTK - FaceWarrior - Warrior Mech - Mage Mech -

using System.Linq;

namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {
        //Init value for Card Draw
        private int EnemyCardDrawValue = 2;
        private int FriendCardDrawValue = 2;

        //Init value for heros
        private int HeroEnemyHealthValue = 3;
        private int HeroFriendHealthValue = 2;

        //Init value for minion
        private int MinionEnemyAttackValue = 3;
        private int MinionEnemyHealthValue = 2;
        private int MinionFriendAttackValue = 2;
        private int MinionFriendHealthValue = 2;

        //GlobalValueModifier
        private int GlobalValueModifier;

        public override float GetBoardValue(Board board)
        {
            float value = 0;

            //Change base value according to the deck played and situation
            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.PaladinSecret)
            {
                //Debug("Secretatin");
                FriendCardDrawValue = 6;
                EnemyCardDrawValue = 0;
                HeroEnemyHealthValue = 3;
                HeroFriendHealthValue = 1;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 3;
                MinionFriendHealthValue = 2;
                if (board.EnemyClass == Card.CClass.HUNTER) //Against hunter, save more health and little more control
                {
                    HeroFriendHealthValue = 4;
                    MinionEnemyAttackValue = 5;
                }
                else //Against other Hero than hunter
                {
                    if (board.TurnCount <= 4) //Little more control before turn 4
                        MinionEnemyAttackValue = 4;
                }
            }
            else if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechMage
                     || ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechWarrior)
            {
                //Debug("Mech");
                FriendCardDrawValue = 5;
                EnemyCardDrawValue = 6;
                HeroEnemyHealthValue = 3;
                HeroFriendHealthValue = 1;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 3;
                MinionFriendHealthValue = 2;
                if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechMage)
                {
                    if (board.TurnCount <= 4) //Little more control before turn 4
                        MinionEnemyAttackValue = 4;
                }
            }
            else if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.Zoo)
            {
                //Debug("Zoo");
                FriendCardDrawValue = 2;
                EnemyCardDrawValue = 3;
                HeroEnemyHealthValue = 2;
                HeroFriendHealthValue = 2;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 2;
                MinionFriendHealthValue = 2;
                if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor <= 12)
                {
                    if (board.EnemyClass == Card.CClass.HUNTER)
                        HeroFriendHealthValue = 29;
                    else
                        HeroFriendHealthValue = 7;
                }
            }
            else if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
            {
                Debug("worgenotk");
                FriendCardDrawValue = 15;
                EnemyCardDrawValue = 2;
                HeroEnemyHealthValue = 2;
                HeroFriendHealthValue = 2;
                MinionEnemyAttackValue = 4;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 3;
                MinionFriendHealthValue = 2;
                if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor <= 15)
                {
                    HeroFriendHealthValue = 7;
                }
            }
            else if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
            {
                //Debug("MidHunter");
                FriendCardDrawValue = 2;
                EnemyCardDrawValue = 4;
                HeroEnemyHealthValue = 3;
                HeroFriendHealthValue = 2;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 2;
                MinionFriendHealthValue = 2;
            }
            else
            {
                Debug("Warning ! Your deck is not detected. Please play a supported deck.");
            }

            
            //Hero friend value
            value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor)*HeroFriendHealthValue;

            //Hero enemy value
            value -= (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor)*HeroEnemyHealthValue;

            //enemy board
            foreach (Card c in board.MinionEnemy)
                value -= GetCardValue(board, c);

            //friend board
            foreach (Card c in board.MinionFriend)
                value += GetCardValue(board, c);

            value += GlobalValueModifier;

            //Lethal and save my ass
            if (board.HeroEnemy.CurrentHealth <= 0)
                value += 100000;

            if (board.HeroFriend.CurrentHealth <= 0 && board.FriendCardDraw == 0)
                value -= 100000;

            value += board.FriendCardDraw*FriendCardDrawValue;
            value -= board.EnemyCardDraw*EnemyCardDrawValue;

            //Some secret rules
            if (board.Secret.Count(x => x.Template.Id == Card.Cards.EX1_611) == 1) //If we have a freezing trap on board
            {
                if (board.MinionEnemy.Count == 1)
                {
                    foreach (Card card in board.MinionEnemy)
                    {
                        if (GetCardValue(board,card) > 17) //inscreave value of the board if there is only big creature on the enemy board
                        {
                            GlobalValueModifier += 5;
                        }
                    }
                }
            }

            if (board.Secret.Count(x => x.Template.Id == Card.Cards.EX1_610) == 1) //If we have a explosive trap on board
            {
                if (board.MinionEnemy.Count < 2)
                {
                    GlobalValueModifier -= 4;
                }
            }

            //Setup Lethal
            if (board.EnemyClass == Card.CClass.WARRIOR || board.EnemyClass == Card.CClass.PRIEST)
            {
                if ((board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor + 2) <= GetDmgInHand(board))
	            {
                    GlobalValueModifier += 20;
	            }
            }
            else if (board.EnemyClass == Card.CClass.DRUID && (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor + 1) <= GetDmgInHand(board))
            {
                GlobalValueModifier += 20;
            }
            else if ((board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) <= GetDmgInHand(board))
            {
                GlobalValueModifier += 20;
            }
                
            return value;
        }

        //Uncomment this only for Debug wierd play
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
                value += card.CurrentHealth*MinionFriendHealthValue + card.CurrentAtk*MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 2 + card.CurrentAtk;

                if (card.IsDivineShield)
                    value += 2 + card.CurrentAtk;

                if (card.IsStealth)
                    value += 1 + card.CurrentAtk;

                if (card.IsTargetable == false)
                    value += 3;

                //Tweak some minions Friend value
                switch (card.Template.Id)
                {
                    case Card.Cards.FP1_007: //Nerubian Egg
                        if (card.IsTaunt)
                            value += 5;
                        break;
                }
            }
            else
            {
                //Taunt value
                if (card.IsTaunt)
                    value += 2;

                //Windfury value
                if (card.IsWindfury)
                    value += 4 + card.CurrentAtk * 2;

                //Divine shield value
                if (card.IsDivineShield)
                    value += 2 + card.CurrentAtk;

                //Targetable Value
                if (card.IsTargetable == false)
                    value += 3;

                //Frozen Value
                if (card.IsFrozen)
                    value -= 1 + card.CurrentAtk;

                value += card.CurrentHealth*MinionEnemyHealthValue + card.CurrentAtk*MinionEnemyAttackValue;

                //Tweak some minions enemy value
                switch (card.Template.Id)
                {
                    case Card.Cards.EX1_412: //Raging Worgen
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.EX1_565: //Flametongue Totem
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.BRM_002: //Flamewaker
                        if (card.IsSilenced == false)
                            value += 12;
                        break;

                    case Card.Cards.EX1_402: //Armorsmith
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.GVG_006: //Mechwarper
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.FP1_005: //Shade of Naxxramas
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.FP1_013: //Kel'Thuzad
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.BRM_031: //Chromaggus
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_559: //Archmage Antonidas
                        if (card.IsSilenced == false)
                            value += 15;
                        break;

                    case Card.Cards.GVG_021: //Mal'Ganis
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_608: //Sorcerer's Apprentice
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.NEW1_012: //Mana Wyrm
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_595: //Cult Master
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_170: //Emperor Cobra
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.BRM_028: //Emperor Thaurissan
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.GVG_100: //Floating Watcher
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.tt_004: //Flesheating Ghoul
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_604: //Frothing Berserker
                        if (card.IsSilenced == false)
                            value += 20;
                        break;

                    case Card.Cards.BRM_019: //Grim Patron
                        if (card.IsSilenced == false && card.CurrentHealth > 1)
                            value += 10;
                        break;

                    case Card.Cards.EX1_084: //Warsong Commander
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_095: //Gadgetzan Auctioneer
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.NEW1_040: //Hogger
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.GVG_104: //Hobgoblin
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_614: //Illidan Stormrage
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.GVG_027: //Iron Sensei
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.GVG_094: //Jeeves
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.NEW1_019: //Knife Juggler
                        if (card.IsSilenced == false)
                            value += 9;
                        break;

                    case Card.Cards.EX1_001: //Lightwarden
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_563: //Malygos
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.GVG_103: //Micro Machine
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_044: //Questing Adventurer
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.NEW1_020: //Wild Pyromancer
                        if (card.IsSilenced == false)
                            if (board.MinionFriend.Count >= 2)
                            {
                                value += 5;
                            }
                        break;

                    case Card.Cards.GVG_013: //Cogmaster
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.CS2_237: //Starving Buzzard
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.EX1_080: //Secretkeeper
                        if (card.IsSilenced == false)
                            value += 7;
                        break;

                    case Card.Cards.GVG_002: //Snowchugger
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.CS2_235: //Northshire Cleric
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                            if (card.CurrentHealth < card.MaxHealth)
                            {
                                value += 9;
                            }
                        }
                        break;

                    case Card.Cards.AT_076: //Murloc Knight
                        if (card.IsSilenced == false)
                            value += 10;
                        break;

                    case Card.Cards.AT_038: //Darnassus Aspirant
                        if (card.IsSilenced == false && board.TurnCount < 7)
                            value += 5;
                        break;

                    case Card.Cards.EX1_055: //Mana Addict
                        if (card.IsSilenced == false)
                            value += 5;
                        break;

                    case Card.Cards.EX1_572: //Ysera
                        if (card.IsSilenced == false)
                            value += 15;
                        break;
                }
            }

            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.FP1_004: //Mad Scientist
                        GlobalValueModifier += 4;
                    break;

                case Card.Cards.EX1_412://Raging Worgen
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 60;
                        if (board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_103) >= 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_607) >= 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_104) >= 1 && board.ManaAvailable >= 8 && board.MinionEnemy.Count(x => x.IsTaunt) == 0) //We check if we have the 4 combo card in hand. If yes, we cast 20 dmg.
                        {
                            if (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor > 32) //If heroenemy has more than 32hp, we start doing damage
                            {
                                GlobalValueModifier += 25;
                            }
                            else
                            {
                                //Hero has 30hp max, we wait the total combo to kill him
                            }
                        }
                    }
                    break;

                case Card.Cards.EX1_096://Loot Hoarder
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier += 12;
                    }
                    break;

                case Card.Cards.AT_087://Argent Horserider
                        GlobalValueModifier -= 7;
                    break;

                case Card.Cards.FP1_012://Sludge Belcher
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier += 16;
                    }
                    break;

                case Card.Cards.GVG_076://Explosive Sheep
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 9;
                    }
                    break;

                case Card.Cards.EX1_007://Acolyte of Pain
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier += 2;
                    }
                    break;

		        case Card.Cards.EX1_603://Cruel Taskmaster
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 8;
                        if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_603).Count == 2) //If we have 2 taskmaster, increase cast value
                        {
                            GlobalValueModifier += 4;
                        }
                    }
                    break;

                case Card.Cards.AT_079: //Mysterious Challenger
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.PaladinSecret)
                        GlobalValueModifier += 10;
                    break;

                case Card.Cards.CS2_188: //Abusive Sergeant
                    GlobalValueModifier -= 8;
                    if (board.MinionEnemy.Count == 0) //Avoid play abusive if empty enemy board (use it for trade)
                        GlobalValueModifier -= 4;
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
                    {
                        if (target != null)
                        {
                            if (target.IsFriend == false || target.CanAttack == false)
                            {
                                GlobalValueModifier -= 18;
                            }
                        }
                    }
                    break;

                case Card.Cards.FP1_002: //Haunted Creeper
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.NEW1_019) >= 1 && board.ManaAvailable <= 3)
                        GlobalValueModifier += 7;
                    break;

                case Card.Cards.CS2_203: //Ironbeak Owl
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 18;
                    }
                    else
                    {
                        GlobalValueModifier -= 15;
                    }
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter && target != null && target.IsTaunt)
                    {
                        GlobalValueModifier += 5;
                    }
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_203) == 2) //If wa have 2 owl, increase cast value
                            GlobalValueModifier += 10;

                        if (board.EnemyClass == Card.CClass.HUNTER) //Inscrease value of owl against hunter
                        {
                            if (target != null && target.IsFriend == false)
                                GlobalValueModifier += 10;

                            if (target != null && target.Template.Id == Card.Cards.FP1_004)
                                //Inscrease value of owl against mad scientitst
                                GlobalValueModifier += 3;
                        }
                    if (target != null && target.Template.Id == Card.Cards.NEW1_021 && target.IsSilenced == false)
                        //Inscrease value of owl against doomsayer
                        GlobalValueModifier += 110;
                    break;

                case Card.Cards.AT_076: //Murloc Knight
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.GVG_013: //Cogmaster
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.GVG_051) >= 1)
                        GlobalValueModifier += 3;
                    if (board.TurnCount == 1)
                    {
                        GlobalValueModifier += 2;
                    }
                    break;

                case Card.Cards.GVG_102: //Tinkertown Technician
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                        GlobalValueModifier -= 4;
                    break;

                case Card.Cards.GVG_055: //Screwjank Clunker
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.GVG_085: //Annoy-o-Tron
                    if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.GVG_013) >= 1)
                        GlobalValueModifier += 3;
                    break;

                case Card.Cards.GVG_004: //Goblin Blastmage
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                        GlobalValueModifier -= 6;
                    break;

                case Card.Cards.GVG_016: //Fel Reaver
                        GlobalValueModifier -= 2;
                    break;

                case Card.Cards.EX1_116: //Leeroy Jenkins
                    GlobalValueModifier -= 30;
                    break;

                case Card.Cards.EX1_319: //Flame Imp
                    if (board.TurnCount == 1)
                        GlobalValueModifier += 5;
                    break;

                case Card.Cards.EX1_310: //Doomguard
                    GlobalValueModifier -= 35;
                    if (board.Hand.Count <= 2)
                    {
                        GlobalValueModifier += 9;
                        if (board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_316) == 1 && board.ManaAvailable >= 6)
                            GlobalValueModifier -= 30;
                    }
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 2)
                        GlobalValueModifier += 8;
                    break;

                case Card.Cards.FP1_007: //Nerubian Egg
                    GlobalValueModifier += 6;
                    break;

                case Card.Cards.EX1_162: //Dire Wolf Alpha
                    if (board.MinionFriend.Count == 0)
                        GlobalValueModifier -= 2;
                    break;

                case Card.Cards.EX1_304: //Void Terror
                    GlobalValueModifier -= 10;
                    if (board.MinionFriend.Count == 0 && board.TurnCount > 3)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.CS2_064: //Dread Infernal
                    GlobalValueModifier -= 4;
                    break;

                case Card.Cards.FP1_022: //Voidcaller
                    if (board.Hand.Count(x => x.Race == Card.CRace.DEMON) > 1)
                        GlobalValueModifier += 5;
                    break;

                case Card.Cards.AT_122: //Gormok the Impaler
                    if (board.MinionFriend.Count < 4)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.NEW1_019: //Knife Juggler
                    if (CanSurvive(minion, board) == false || board.EnemyClass == Card.CClass.ROGUE)
                        GlobalValueModifier -= 2;
                    if (CanSurvive(minion,board))
                    {
                        GlobalValueModifier += 2;
                    }
                    break;

                case Card.Cards.EX1_093: //Defender of Argus
                    GlobalValueModifier -= 3;
                    break;

                case Card.Cards.EX1_089: //Arcane Golem
                    GlobalValueModifier -= 17;
                    break;

                case Card.Cards.GVG_069://Antique Healbot
                    if (board.HeroFriend.CurrentHealth > 23)
                    {
                        GlobalValueModifier -= 5;
                    }
                    GlobalValueModifier -= 15;
                    break;
            }

            foreach (Card card in board.MinionEnemy)
            {
                if (card.Template.Id == Card.Cards.NEW1_021 && card.IsSilenced == false) //Tweak Doomsayer
                    GlobalValueModifier -= 100;
            }
        }

        public override void OnMinionDeath(Board board, Card minion)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.EX1_096: //Loot Hoarder
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                        if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                        {
                            GlobalValueModifier -= 15;
                        }
                    break;

                case Card.Cards.FP1_004: //Mad Scientist
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier -= 20;
                    break;

                case Card.Cards.FP1_022: //Voidcaller
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier -= 5;
                    if (minion.IsFriend && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 5;
                        if (board.Hand.Count(x => x.Race == Card.CRace.DEMON) == 1 &&
                            board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 1)
                            //Some tweak here for higher chance to proc doomguard
                            GlobalValueModifier += 5;
                    }
                    break;

                case Card.Cards.EX1_029: //Leper Gnome
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier -= 6;
                    if (minion.IsFriend && minion.IsSilenced == false)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.FP1_002: //Haunted Creeper
                    if (minion.IsFriend && minion.IsSilenced == false)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.GVG_096: //Piloted Shredder
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier += 3;
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                        GlobalValueModifier -= 3;
                    break;

                case Card.Cards.EX1_534: //Savannah Highmane
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.FP1_007: //Nerubian Egg
                    if (minion.IsFriend && minion.IsSilenced == false && minion.IsTaunt)
                        GlobalValueModifier -= 9;
                    break;

                case Card.Cards.EX1_556: //Harvest Golem
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier += 5;
                    break;

                case Card.Cards.AT_123: //Chillmaw
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier += 13;
                    break;

                case Card.Cards.NEW1_021: //Doomsayer
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                        GlobalValueModifier += 1000;
                    break;
            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {

                case Card.Cards.NEW1_031://Animal Companion
                    GlobalValueModifier += 7;
                    break;

                case Card.Cards.CS2_084://Hunter's Mark
                    GlobalValueModifier -= 10;
                    break;

                case Card.Cards.CS2_108://Execute
                    GlobalValueModifier -= 21;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_108).Count == 2)
                    {
                        GlobalValueModifier += 5;
                    }
                    break;

                case Card.Cards.CS2_103://Charge
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 70;
                        if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.EX1_412) == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_607) >= 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_104) >= 1 && target.Template.Id == Card.Cards.EX1_412)
                        {
                            //Debug("worgen trouvé sur le board phase 1");
                            GlobalValueModifier += 50;
                            if (board.FriendGraveyard.FindAll(x => x == Card.Cards.CS2_103).Count == 1)
                            {
                                //Debug("Une charge d'utilisé, on doit garder la 2e");
                                GlobalValueModifier -= 25;
                            }
                        }
                    }
                    break;

                case Card.Cards.EX1_607://Inner Rage
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 106;
                        if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.EX1_412) == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_104) >= 1 && target.Template.Id == Card.Cards.EX1_412)
                        {
                            //Debug("worgen trouvé sur le board phase 2");
                            GlobalValueModifier += 100;
                            if (board.FriendGraveyard.Count(x => x == Card.Cards.EX1_607) == 1)
                            {
                                //Debug("Une inner rage d'utilisé, on doit garder la 2e");
                                GlobalValueModifier -= 50;
                            }
                        }
                    }
                    break;

                case Card.Cards.CS2_104://Rampage
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                    {
                        GlobalValueModifier -= 45;
                        if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.EX1_412) == 1 && target.Template.Id == Card.Cards.EX1_412 && target.CanAttack)
                        {
                            //Debug("worgen trouvé sur le board phase 3");
                            GlobalValueModifier += 40;
                        }
                    }
                    break;

                case Card.Cards.EX1_410://Shield Slam
                    GlobalValueModifier -= 12;
                    if (target.IsFriend)//Avoid to target the acolyte or loot harder
                    {
                        GlobalValueModifier -= 19;
                    }
                    break;

                case Card.Cards.EX1_407://Brawl
                    GlobalValueModifier -= 21;
                    break;

                case Card.Cards.EX1_391://Slam
                    GlobalValueModifier -= 10;
                    if (target.IsFriend)//Avoid to target friend to draw
                    {
                        GlobalValueModifier -= 2;
                    }
                    break;

                case Card.Cards.EX1_606://Shield Block
                    GlobalValueModifier -= 25;
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_410) == 1 && board.ManaAvailable > 3 || board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_606) > 1)//If we have shield slam, or if we have 2 shield block, increase value
                    {
                        GlobalValueModifier += 15;
                        //Debug("On monte la value car on a la combo shield block + shield slam. Mana dispo : " + board.ManaAvailable.ToString());
                    }
                    break;

                case Card.Cards.EX1_400://Whirlwind
                    GlobalValueModifier -= 13;
                    break;

                case Card.Cards.EX1_349: //Divine Favor
                    if ((board.Hand.Count > board.EnemyCardCount))
                        GlobalValueModifier -= 6;
                    break;

                case Card.Cards.AT_073: //Competitive Spirit
                    if (board.MinionFriend.Count <= 1)
                        GlobalValueModifier -= 5;
                    else
                        GlobalValueModifier += 2;
                    break;

                case Card.Cards.CS2_093: //Consecration
                    if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.PaladinSecret)
                        GlobalValueModifier -= 11;
                    break;

                case Card.Cards.CS2_092: //Blessing of Kings
                    if (target.IsDivineShield && target.IsFriend)
                        GlobalValueModifier += 1;
                    break;

                case Card.Cards.EX1_379: //Repentance
                    if (board.TurnCount == 1)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.EX1_136: //Redemption
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.EX1_130: //Noble Sacrifice
                    if (board.MinionFriend.Count == 0)
                        GlobalValueModifier -= 3;
                    break;

                case Card.Cards.GVG_061: //Muster for Battle
                    GlobalValueModifier -= 2;
                    if (board.WeaponEnemy != null && board.WeaponEnemy.CurrentDurability <= 1 &&
                        board.WeaponEnemy.Template.Id == Card.Cards.FP1_021)
                        GlobalValueModifier -= 8;
                    if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 &&
                        board.WeaponFriend.Template.Id == Card.Cards.CS2_097)
                        GlobalValueModifier -= 5;
                    if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability > 1 &&
                        board.WeaponFriend.Template.Id == Card.Cards.EX1_383t)
                        GlobalValueModifier -= 13;
                    break;

                case Card.Cards.CS2_105: //Heroic Strike
                    GlobalValueModifier -= 9;
                    break;

                case Card.Cards.EX1_277: //Arcane Missiles
                    GlobalValueModifier -= 3;
                    break;

                case Card.Cards.EX1_408: //Mortal Strike
                    GlobalValueModifier -= 16;
                    if (target != board.HeroEnemy || target.IsTaunt == false)
                        GlobalValueModifier -= 5;
                    break;

                case Card.Cards.GVG_003: //Unstable Portal
                    GlobalValueModifier += 5;
                    break;

                case Card.Cards.CS2_029: //Fireball
                    GlobalValueModifier -= 19;
                    if (target == board.HeroEnemy)
                    {
                        GlobalValueModifier -= 2;
                    }
                    break;

                case Card.Cards.CS2_024: //Frostbolt
                    GlobalValueModifier -= 6;
                    break;

                case Card.Cards.AT_005: //Polymorph: Boar
                    if (target.IsFriend)
                        GlobalValueModifier -= 15;
                    else
                        GlobalValueModifier -= 12;
                    break;

                case Card.Cards.EX1_316: //Power Overwhelming
                    GlobalValueModifier -= 9;
                    if (target.CanAttack == false)
                        GlobalValueModifier -= 18;
                    if (board.Hand.Count <= 2)
                    {
                        bool hasDoomGuardInHand = (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.FP1_007) >=
                                                   1 && board.ManaAvailable >= 6);
                        foreach (Card card in board.Hand)
                        {
                            if (card.Template.Id == Card.Cards.EX1_310 && hasDoomGuardInHand)
                                //We have doomgard and PO in hand, egg on board, use PO before doomguard
                                GlobalValueModifier += 15;
                        }
                    }
                    if (board.MinionEnemy.Count == 0) //Avoid play PO if empty enemy board (use it for trade)
                        GlobalValueModifier -= 3;
                    break;

                case Card.Cards.EX1_320: //Bane of Doom
                    if (target.CurrentHealth <= 2 && board.MinionFriend.Count < 7)
                        GlobalValueModifier += 10;
                    else
                        GlobalValueModifier -= 10;
                    break;

                case Card.Cards.GVG_045: //Imp-losion
                    GlobalValueModifier -= 13;
                    if (target.IsFriend)
                        GlobalValueModifier -= 8;

                    if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.NEW1_019) >= 1)
                        GlobalValueModifier += 10;

                    if (board.WeaponEnemy != null && board.WeaponEnemy.CurrentDurability <= 1 &&
                        board.WeaponEnemy.Template.Id == Card.Cards.FP1_021)
                        GlobalValueModifier -= 8;
                    break;

                case Card.Cards.BRM_005: //Demonwrath
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.EX1_302: //Mortal Coil
                    GlobalValueModifier -= 5;
                    if (target.IsFriend)
                        GlobalValueModifier -= 7;
                    if (target.CurrentHealth == 1)
                        GlobalValueModifier += 4;
                    break;

                case Card.Cards.EX1_596: //Demonfire
                    GlobalValueModifier -= 9;
                    if (target.IsFriend && target.CanAttack)
                        GlobalValueModifier += 4;

                    break;

                case Card.Cards.GVG_015: //Darkbomb
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.CS2_062: //Hellfire
                    GlobalValueModifier -= 11;
                    break;

                case Card.Cards.EX1_539: //Kill Command
                    GlobalValueModifier -= 15;
                    break;

                case Card.Cards.BRM_013: //Quick Shot
                    GlobalValueModifier -= 10;
                    if (target == board.HeroEnemy && board.Hand.Count > 1)
                    {
                        GlobalValueModifier -= 10;
                    }
                    break;

                case Card.Cards.EX1_538: //Unleash the Hounds
                    GlobalValueModifier -= 22;
                    //if (board.MinionEnemy.Count < 2)
                    //{
                    //    GlobalValueModifier -= 8;
                    //}
                    break;

                case Card.Cards.EX1_544: //Flare
                    if (board.EnemyClass == Card.CClass.MAGE || board.EnemyClass == Card.CClass.PALADIN || board.EnemyClass == Card.CClass.HUNTER)
                    {
                        GlobalValueModifier -= 5;
                        if (board.EnemyClass == Card.CClass.PALADIN)
                        {
                            if (board.SecretEnemyCount > 1)
                            {
                                GlobalValueModifier += 15;
                            }
                        }
                        else
                        {
                            if (board.SecretEnemy)
                            {
                                GlobalValueModifier += 15;
                            }
                        } 
                    }
                    break;

                case Card.Cards.PART_001: //Armor Plating
                    GlobalValueModifier -= 2;
                    break;

                case Card.Cards.PART_002: //Time Rewinder
                    GlobalValueModifier -= 2;
                    break;

                case Card.Cards.PART_003: //Rusty Horn
                    GlobalValueModifier -= 2;
                    break;

                case Card.Cards.PART_004: //Finicky Cloakfield
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.PART_005: //Emergency Coolant
                    GlobalValueModifier -= 2;
                    break;

                case Card.Cards.PART_006: //Reversing Switch
                    GlobalValueModifier -= 2;
                    if (target.CanAttack == false && target.IsFriend)
                        GlobalValueModifier -= 4;
                    break;

                case Card.Cards.PART_007: //Whirling Blades
                    GlobalValueModifier -= 2;
                    break;

                case Card.Cards.GAME_005: //The Coin
                    GlobalValueModifier -= 4;
                    break;
            }

            if (spell.Template.IsSecret)
            {
                if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.PaladinSecret)
                {
                    GlobalValueModifier += 5;
                }

                if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
                {
                    GlobalValueModifier += 7;
                }
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
            if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 &&
                board.WeaponFriend.Template.Id != Card.Cards.CS2_091)
                GlobalValueModifier -= 6;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                GlobalValueModifier += 1;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechWarrior)
                GlobalValueModifier += 2;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
                GlobalValueModifier += 3;

            if (weapon.Template.Id == Card.Cards.GVG_043)//Glaivezooka
            {
                if (board.MinionFriend.Count == 0)
                {
                    GlobalValueModifier -= 16;
                }
                else if (board.MinionFriend.Count(x => x.CanAttack) == board.MinionFriend.Count)
                {
                    GlobalValueModifier += 2;
                }
                    
            }
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {
            if (board.WeaponFriend != null && attacker.Type == Card.CType.WEAPON)
            {
                if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
                {
                    if (board.WeaponFriend.Template.Id == Card.Cards.EX1_536 && board.WeaponFriend.CurrentDurability == 1 && board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count == 0)
                    {
                        //Don't attack with an eaglehorn bow with 1 durability
                        GlobalValueModifier -= 15;
                    }
                }

                if (attacker.Type == Card.CType.WEAPON && target.Type == Card.CType.HERO &&
                    board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count == 0 &&
                    board.WeaponFriend.CurrentDurability == 1 &&
                    ArchetypeManager.GetFriendlyArchetype(board) != ArchetypeManager.Archetype.MidHunter)
                    //If we have a weapon with 1 durability and no other weapon in hand, avoid to attack the HERO
                    GlobalValueModifier -= 15;

                if (board.WeaponFriend.Template.Id == Card.Cards.FP1_021 && board.WeaponFriend.CurrentDurability == 1 &&
                    target.CurrentHealth == 1 && !target.IsTaunt)
                    //If Death's Bite durability is 1, avoid to directly attack 1 health minion (unless if its taunt)
                    GlobalValueModifier -= 46;

                if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.WorgenOtk)
                {
                    if (target == board.HeroEnemy)
                    {
                        GlobalValueModifier -= 2;
                    }
                }
            }


            if (target.Template.Id == Card.Cards.EX1_007 && target.IsFriend == false)
                //Try to one shot acolyte to avoid enemy draw
            {
                if (attacker.CurrentAtk >= target.CurrentHealth)
                {
                    GlobalValueModifier += 4;
                    if (board.EnemyClass == Card.CClass.WARRIOR)
                        GlobalValueModifier += 4;
                }
            }
        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.AT_076) >= 1)
                GlobalValueModifier += 5;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechWarrior || ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MechMage)
                GlobalValueModifier += 1;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.MidHunter)
                GlobalValueModifier -= 2;

            if (ArchetypeManager.GetFriendlyArchetype(board) == ArchetypeManager.Archetype.Zoo)
            {
                GlobalValueModifier += 3;
                if (board.Hand.Count == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 1)
                    //Avoid HeroPower if Doomguard in hand and alone
                    GlobalValueModifier -= 3;

                if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.FP1_022) >= 1 &&
                    board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) >= 1)
                    //Avoid HeroPower if we have Voidcaller on board and Doomguard in Hand
                    GlobalValueModifier -= 3;
            }

            if (board.TurnCount == 1) //Avoid coin Heropower
                GlobalValueModifier += 7;
        }

        public void OnProcessAction(Action a, Board board)
        {
            float moveVal = board.TrapMgr.GetSecretModifier(a, board, true);
            GlobalValueModifier += (int) moveVal;
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
            return false;
            //Debug("False");
        }

        public int GetTotalEnemyDmg(Board board)
        {
            int i = 0;

            foreach (Card card in board.MinionEnemy)
                i = card.CurrentAtk + i;

            i = board.HeroEnemy.CurrentAtk + i;

            return i;
        }

        public int GetDmgInHand(Board board)
        {
            int iw = 0;
            int i = 0;

            foreach (Card card in board.Hand)
            {
                if (card.Type == Card.CType.WEAPON) //Weapons
                {
                    if (card.CurrentAtk >= iw)
                        iw = card.CurrentAtk;
                }
                if (card.Template.Id == Card.Cards.NEW1_011) //Kor'kron Elite
                    i += 4;
                if (card.Template.Id == Card.Cards.CS2_105) //Heroic strike
                    i += 4;
                if (card.Template.Id == Card.Cards.EX1_408) //Mortal Strike
                {
                    if (board.HeroFriend.CurrentHealth < 12)
                        i += 6;
                    else
                        i += 4;
                }
                if (card.Template.Id == Card.Cards.CS2_029) //Fireball
                    i += 6;
                if (card.Template.Id == Card.Cards.CS2_024) //Frostbolt
                    i += 3;
                if (card.Template.Id == Card.Cards.BRM_013) //Quick Shot
                    i += 3;
                if (card.Template.Id == Card.Cards.EX1_539) //Kill command
                    i += 3;
            }
            if (board.FriendClass == Card.CClass.HUNTER)
            {
                i += 2;
            }
            if (board.WeaponFriend != null)
                i += board.HeroFriend.CurrentAtk;
            //else
            //    i += iw; //We add weapon hand dmg

            //Debug("We have " + i + "dmg in hand");
            return i;
        }

        public static class ArchetypeManager
        {
            public enum Archetype
            {
                None,
                PaladinSecret,
                Zoo,
                MechWarrior,
                MechMage,
                WorgenOtk,
                FaceWarrior,
                PriestControl,
                Handlock,
                FreezeMage,
                MidHunter
            }

            private static bool _archetypeSetup;
            private static Archetype _archetypeFriendly;
            private static Archetype _archetypeEnemy;


            public static Archetype GetFriendlyArchetype(Board board)
            {
                if (!_archetypeSetup)
                    SetupArchetype(board);

                return _archetypeFriendly;
            }

            public static Archetype GetEnemyArchetype(Board board)
            {
                if (!_archetypeSetup)
                    SetupArchetype(board);

                return _archetypeEnemy;
            }

            private static void SetupArchetype(Board board)
            {
                if (isPaladinSecret(board))
                    _archetypeFriendly = Archetype.PaladinSecret;
                else if (isZoo(board))
                    _archetypeFriendly = Archetype.Zoo;
                else if (isMechWarrior(board))
                    _archetypeFriendly = Archetype.MechWarrior;
                else if (isMechMage(board))
                    _archetypeFriendly = Archetype.MechMage;
                else if (isFaceWarrior(board))
                    _archetypeFriendly = Archetype.FaceWarrior;
                else if (isPriestControl(board))
                    _archetypeFriendly = Archetype.PriestControl;
                else if (isMidHunter(board))
                    _archetypeFriendly = Archetype.MidHunter;
                else if (isWorgenOtk(board))
                    _archetypeFriendly = Archetype.WorgenOtk;


                if (isHandLock(board))
                    _archetypeEnemy = Archetype.Handlock;
                else if (isFreezeMage(board))
                    _archetypeEnemy = Archetype.FreezeMage;


                _archetypeSetup = true;
            }

            public static bool isPaladinSecret(Board board)
            {
                if (board.FriendClass == Card.CClass.PALADIN &&
                    board.Deck.Count(x => CardTemplate.LoadFromId(x).Id == Card.Cards.AT_079) >= 1)
                {
                    return true;
                }
                return false;
            }

            public static bool isMidHunter(Board board)
            {
                if (board.FriendClass == Card.CClass.HUNTER)
                {
                    return true;
                }
                return false;
            }

            public static bool isZoo(Board board)
            {
                if (board.FriendClass == Card.CClass.WARLOCK)
                    return true;
                return false;
            }

            public static bool isMechWarrior(Board board)
            {
                if (board.FriendClass == Card.CClass.WARRIOR &&
                    board.Deck.Count(x => CardTemplate.LoadFromId(x).Race == Card.CRace.MECH) >= 10)
                {
                    return true;
                }
                return false;
            }

            public static bool isMechMage(Board board)
            {
                if (board.FriendClass == Card.CClass.MAGE &&
                    board.Deck.Count(x => CardTemplate.LoadFromId(x).Race == Card.CRace.MECH) >= 10)
                {
                    return true;
                }
                return false;
            }

            public static bool isWorgenOtk(Board board)
            {
                if (board.FriendClass == Card.CClass.WARRIOR && board.Deck.Count(x => CardTemplate.LoadFromId(x).Id == Card.Cards.EX1_412) >= 1)
                {
                    return true;
                }
                return false;
            }

            public static bool isFaceWarrior(Board board)
            {
                //if (board.FriendClass == Card.CClass.WARRIOR)
                //{
                //    return true;
                //}
                return false;
            }

            public static bool isPriestControl(Board board)
            {
                if (board.FriendClass == Card.CClass.PRIEST)
                {
                    return true;
                }
                return false;
            }

            public static bool isHandLock(Board board)
            {
                //Check if opponent has played some handlock card (check graveyard)
                if (board.TurnCount == 3 && board.EnemyGraveyard.Count == 0 &&
                    board.HeroEnemy.Template.Id == Card.Cards.HERO_07)
                    return true;
                return false;
            }

            public static bool isFreezeMage(Board board)
            {
                if (board.EnemyGraveyard.Count(x => x == Card.Cards.NEW1_021) >= 1 &&
                    board.HeroEnemy.Template.Id == Card.Cards.HERO_08)
                    return true;
                return false;
            }
        }
    }
}