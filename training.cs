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

        public override void OnBoardReady(Board board)
        {
            if(board.IsOwnTurn)
            {
                foreach (var item in board.ActionsStack)
            {
            Debug(item.ToString());
            }

            Debug("Board : " + board.GetValue());
            }
        }

        public float GetCardValue(Board board, Card card)
        {
            float value = 0;

            if (card.IsFriend)
            {
                value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 5;
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

            ret._logBestMove.AddRange(_logBestMove);
            ret._log = _log;

            return ret;
        }
    }
}