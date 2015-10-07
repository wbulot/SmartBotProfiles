/*Smarbot Profile
* Deck from : http://www.hearthpwn.com/decks/242748-worgen-30-dmg-otk
* Contributors : Wbulot
*/


namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {

        private int MinionEnemyTauntValue = 1;
        private int MinionEnemyWindfuryValue = 6;

        private int FriendCardDrawValue = 18;
        private int EnemyCardDrawValue = 2;

        private int HeroEnemyHealthValue = 3;
        private int HeroFriendHealthValue = 2;

        private int MinionEnemyAttackValue = 4;
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

        public override float GetBoardValue(Board board)
        {
            float value = 0;

            //if (board.HeroFriend.CurrentHealth < 25)
            //{
            //    HeroFriendHealthValue += 3;
            //}

            //Hero friend value
            if (board.RootBoard.HeroFriend.CurrentHealth + board.RootBoard.HeroFriend.CurrentArmor > 15)
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;
            }
            else
            {
                value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * (HeroFriendHealthValue + 5);
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
            }
            else
            {
                value += GetThreatModifier(card);
                //Taunt value
                if (card.IsTaunt)
                    value += MinionEnemyTauntValue;

                if (card.IsWindfury)
                    value += MinionEnemyWindfuryValue;

                value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;
            }

            //Debug(value.ToString());
            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.EX1_603://Cruel Taskmaster
                    MinionCastGlobalValue -= 8;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_603).Count == 2)
                    {
                        MinionCastGlobalValue += 4;
                        //Debug("2 taskmater, on monte la value");
                    }
                    break;
                case Card.Cards.CS2_203://Ironbeak Owl
                    MinionCastGlobalValue -= 18;
                    break;
                case Card.Cards.EX1_096://Loot Hoarder
                    MinionCastGlobalValue += 12;
                    break;
                case Card.Cards.FP1_012://Sludge Belcher
                    MinionCastGlobalValue += 16;
                    break;
                case Card.Cards.GVG_076://Explosive Sheep
                    MinionCastGlobalValue -= 9;
                    break;
                case Card.Cards.EX1_007://Acolyte of Pain
                    MinionCastGlobalValue += 2;
                    break;
                case Card.Cards.EX1_412://Raging Worgen
                    MinionCastGlobalValue -= 60;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_103).Count >= 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_607).Count >= 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_104).Count >= 1 && board.ManaAvailable >= 8 && board.MinionEnemy.FindAll(x => x.IsTaunt).Count == 0) //On check si on a les 4 cartes de la combo en main, si oui on balance deja au minimum 20dmg
                    {
                        if (board.HeroEnemy.Template.Id == Card.Cards.HERO_01 || board.HeroEnemy.Template.Id == Card.Cards.HERO_01a || board.HeroEnemy.Template.Id == Card.Cards.HERO_06 || board.HeroEnemy.Template.Id == Card.Cards.HERO_08 || board.HeroEnemy.Template.Id == Card.Cards.HERO_08a) //Si le hero est un warrior, un mage, ou un druide, on lance une 1ere combo pour commencer les dégats
                        {
                            MinionCastGlobalValue += 25;
                            //Debug("combo trouvé. C'est un hero qui peut > 30hp. Mana restant : " + board.ManaAvailable.ToString());
                        }
                        else
                        {
                            //Debug("Le Hero a 30 HP max, on attend la combo complète pour le tuer");
                        }
                    }
                    break;
            }
        }

        public override void OnMinionDeath(Board board, Card minion)
        {
            if (minion.Template.Id == Card.Cards.EX1_096)//Avoid to kill Loot Hoarder with aoe
            {
                SpellsCastGlobalValue -= 15;
            }
            if (minion.Template.Id == Card.Cards.EX1_007)//Acolyte of Pain
            {
                GlobalValueModifier -= 5;
            }

            if (minion.Template.Id == Card.Cards.FP1_001 && minion.IsFriend == false && board.HeroFriend.CurrentHealth == 30)//Zombie Chow
            {
                GlobalValueModifier -= 6;
            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {
                case Card.Cards.CS2_108://Execute
                    SpellsCastGlobalValue -= 21;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_108).Count == 2)
                    {
                        //Debug("Double exec en main, on baisse la value, nb execute : " + board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_108).Count.ToString());
                        SpellsCastGlobalValue += 5;
                    }
                    break;
                case Card.Cards.CS2_103://Charge
                    SpellsCastGlobalValue -= 70;
                    if (board.MinionFriend.FindAll(x => x.Template.Id == Card.Cards.EX1_412).Count == 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_607).Count >= 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_104).Count >= 1 && target.Template.Id == Card.Cards.EX1_412)
                    {
                        //Debug("worgen trouvé sur le board phase 1");
                        SpellsCastGlobalValue += 50;
                        if (board.FriendGraveyard.FindAll(x => x == Card.Cards.CS2_103).Count == 1)
                        {
                            //Debug("Une charge d'utilisé, on doit garder la 2e");
                            SpellsCastGlobalValue -= 25;
                        }
                    }
                    break;
                case Card.Cards.EX1_607://Inner Rage
                    SpellsCastGlobalValue -= 106;
                    if (board.MinionFriend.FindAll(x => x.Template.Id == Card.Cards.EX1_412).Count == 1 && board.Hand.FindAll(x => x.Template.Id == Card.Cards.CS2_104).Count >= 1 && target.Template.Id == Card.Cards.EX1_412)
                    {
                        //Debug("worgen trouvé sur le board phase 2");
                        SpellsCastGlobalValue += 100;
                        if (board.FriendGraveyard.FindAll(x => x == Card.Cards.EX1_607).Count == 1)
                        {
                            //Debug("Une inner rage d'utilisé, on doit garder la 2e");
                            SpellsCastGlobalValue -= 50;
                        }
                    }
                    break;
                case Card.Cards.CS2_104://Rampage
                    SpellsCastGlobalValue -= 45;
                    if (board.MinionFriend.FindAll(x => x.Template.Id == Card.Cards.EX1_412).Count == 1 && target.Template.Id == Card.Cards.EX1_412 && target.CanAttack)
                    {
                        //Debug("worgen trouvé sur le board phase 3");
                        SpellsCastGlobalValue += 40;
                    }
                    break;
                case Card.Cards.EX1_410://Shield Slam
                    SpellsCastGlobalValue -= 12;
                    if (target.IsFriend)//Avoid to target the acolyte or loot harder
                    {
                        SpellsCastGlobalValue -= 19;
                    }
                    if (GetCardValue(board,target) < 8)//Avoid to waste the spell against small minion
                    {
                        SpellsCastGlobalValue -= 6;
                    }
                    break;
                case Card.Cards.EX1_407://Brawl
                    SpellsCastGlobalValue -= 21;
                    break;
                case Card.Cards.EX1_391://Slam
                    SpellsCastGlobalValue -= 10;
                    if (target.IsFriend)//Avoid to target friend to draw
                    {
                        SpellsCastGlobalValue -= 2;
                    }
                    break;
                case Card.Cards.EX1_606://Shield Block
                    SpellsCastGlobalValue -= 25;
                    if (board.Hand.FindAll(x => x.Template.Id == Card.Cards.EX1_410).Count == 1 && board.ManaAvailable > 3)//If we have shield slam, increase value
                    {
                        SpellsCastGlobalValue += 15;
                        //Debug("On monte la value car on a la combo shield block + shield slam. Mana dispo : " + board.ManaAvailable.ToString());
                    }
                    break;
                case Card.Cards.EX1_400://Whirlwind
                    SpellsCastGlobalValue -= 13;
                    break;

                case Card.Cards.GAME_005://The Coin
                    SpellsCastGlobalValue -= 4;
                    break;
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
            WeaponCastGlobalValue += 1;
            if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability > 0 )//If we have weapon in hand, decrease value
            {
                //Debug("On a deja une arme en main, on baisse la value");
                WeaponCastGlobalValue -= 13;
            }
            //switch (weapon.Template.Id)
            //{
            //    case Card.Cards.CS2_106://Fiery War Axe
            //        WeaponCastGlobalValue += 0;
            //        break;
            //}

            if (board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count > 1) //If i have another weapon in hand, increave value
            {
                WeaponCastGlobalValue += 9;
                //Debug("On a une autre carte arme weapon en main, on monte la value. Nb arme en main : " + board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count.ToString());
            }

            if (board.MinionEnemy.Count == 1 && weapon.Template.Id == Card.Cards.FP1_021) //If there is only 1 minion on board, and than bot want to cast Death bite
            {
                foreach (Card card in board.MinionEnemy)
                {
                    if (GetCardValue(board, card) <= 8) //if this is a small minion, we decrease the value
                    {
                        WeaponCastGlobalValue -= 18;
                        //Debug("Petite creature seule sur le board, on sort pas la grosse hache pour la tuer");
                    }
                }
            }
            

            if (board.MinionEnemy.Count == 0) //If there is no minion on board, we decrease the value
            {
                WeaponCastGlobalValue -= 20;
            }

            //if (board.MinionEnemy.FindAll(x => x.IsTaunt).Count == 0)//Si il y a aucun taunt sur le board, on se fait rien
            //{
                
            //}
            //else //Si il y a un taunt, on check sa value
            //{
            //    foreach (Card card in board.MinionEnemy)
            //    {
            //        if (card.IsTaunt)
            //        {
            //            if (GetCardValue(board, card) <= 8) 
            //            {
            //                WeaponCastGlobalValue -= 5;
            //                Debug("Taunt, réduit value");
            //            }
            //        }
            //    }
            //}
            
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {

            if (target.Template.Id == Card.Cards.EX1_007 && target.IsFriend == false) //Le but est de minimiser la pioche de l'adversaire, en tuant d'un coup l'acolyte
            {
                if (attacker.CurrentAtk >= target.CurrentHealth)
                {
                    //Debug("On peut one shot l'acolyte : attacker ATK : " + attacker.CurrentAtk + " target Heath : " + target.CurrentHealth);
                    GlobalValueModifier += 4;
                }
            }

            if (attacker.Type == Card.CType.WEAPON && target.Type == Card.CType.HERO && board.MinionEnemy.Count > 0) //If we attack HERO whereas there is still minion on board
            {
                WeaponAttackGlobalCost += 5; //Inscrease value
                foreach (Card c in board.MinionEnemy)
                {
                    if(GetCardValue(board, c) < 11) //If there is a small minion, we increase value to be sure bot will attack him (and not the HERO)
                    {
                        WeaponAttackGlobalCost += 10;
                    }
                }
            }
            if (attacker.Type == Card.CType.WEAPON && board.WeaponFriend != null)
            {
                if (attacker.Type == Card.CType.WEAPON && target.Type == Card.CType.HERO && board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count == 0 && board.WeaponFriend.CurrentDurability == 1) //If we have a weapon with 1 durability, avoid to attack the HERO
                {
                    WeaponAttackGlobalCost += 18;
                }
                if (board.WeaponFriend.Template.Id == Card.Cards.FP1_021 && board.WeaponFriend.CurrentDurability == 1 && target.CurrentHealth == 1 && !target.IsTaunt) //If Death's Bite durability is 1, avoid to directly attack 1 health minion (unless if its taunt)
                {
                    WeaponAttackGlobalCost += 46;
                }
            }
            if (attacker.Type == Card.CType.WEAPON && board.WeaponFriend != null && target.Type == Card.CType.MINION)
            {
                if (true)
                {
                    
                }
            }
            
        }
        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            HeroPowerGlobalCost -= 1;
            if(board.TurnCount == 1) //Avoid coin Heropower
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

            return ret;
        }

        public float GetThreatModifier(Card card)
        {
            switch (card.Template.Id)
            {
                case Card.Cards.GVG_006://Mechwarper
                    return 4;

                case Card.Cards.GVG_096://Piloted Shredder
                    return 6;

                //case Card.Cards.EX1_007://Acolyte of Pain
                //    return 2;

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

                case Card.Cards.NEW1_020://Wild Pyromancer
                    return 5;

                case Card.Cards.GVG_013://Cogmaster
                    return 5;
            }

            return 0;
        }
    }
}