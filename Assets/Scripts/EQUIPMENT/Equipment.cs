/*
 * Class: DrawingWeapon
 * Date: 2020.8.12
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description: 무기를 뺴고 전투에 돌입
*/
namespace HyukinKwon
{
    public static class Equipment
    {
        public enum WEAPON
        {
            ONE_HANDED_SWORD, HALBERD, TWO_HANDED_SWORD
        }

        public enum SHIELD
        {
            NORMAL_SHEILD, HARD_SHEILD
        }

        //모든 무기를 끈다
        public static void Clear(CharacterControl character)
        {
            int len = character.drawedWeapon.Count;
            for (int i = 0; i < len; i++)
            {
                character.drawedWeapon[i].SetActive(false);
                character.undrawedWeapon[i].SetActive(false);
            }
        }
        
        //무기를 조건에 맞게 들고 내린다.
        //Idx 주위
        public static void ToogleWeapon(CharacterControl character)
        {
            if(character.isBattleModeOn)
            {
                character.drawedWeapon[(int)character.weapon].SetActive(true);
                character.undrawedWeapon[(int)character.weapon].SetActive(false);
            }
            else
            {
                character.undrawedWeapon[(int)character.weapon].SetActive(true);
                character.drawedWeapon[(int)character.weapon].SetActive(false);
            }
        }


    }
}
