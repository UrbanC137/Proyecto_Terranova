using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    public GameObject sword; // Referencia al objeto de la espada
    public GameObject axe;   // Referencia al objeto del hacha
    public GameObject pickaxe;  // Referencia al objeto del pico

    private GameObject equippedWeapon = null; // Para rastrear el arma equipada actualmente

    void Start()
    {
        // Desactivar todas las armas al inicio del juego
        if (sword != null)
        {
            sword.SetActive(false);
        }
        if (axe != null)
        {
            axe.SetActive(false);
        }
        if (pickaxe != null)
        {
            pickaxe.SetActive(false);
        }
    }

    void Update()
    {
        // Si se presiona la tecla "1" para equipar la espada
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeaponObject(sword);
        }

        // Si se presiona la tecla "2" para equipar el hacha
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeaponObject(axe);
        }

        // Si se presiona la tecla "3" para equipar el pico
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeaponObject(pickaxe);
        }

        // Si se presiona la tecla "Q" para desequipar cualquier arma
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnequipWeapon();
        }
    }

    // Método para equipar el arma seleccionada
    private void EquipWeaponObject(GameObject weaponToEquip)
    {
        // Si ya hay un arma equipada, la desactivamos
        if (equippedWeapon != null)
        {
            equippedWeapon.SetActive(false);
        }

        // Equipamos el nuevo arma (si no estaba equipada ya)
        if (weaponToEquip != null)
        {
            weaponToEquip.SetActive(true);
            equippedWeapon = weaponToEquip; // Actualizamos la referencia al arma equipada
        }
    }

    // Método para desequipar el arma equipada
    private void UnequipWeapon()
    {
        // Si hay un arma equipada, la desactivamos y eliminamos la referencia
        if (equippedWeapon != null)
        {
            equippedWeapon.SetActive(false);
            equippedWeapon = null; // Ningún arma está equipada ahora
        }
    }
}
