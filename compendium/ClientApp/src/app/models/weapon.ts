import { Attack, HitEffect } from "./monster";

export class WeaponType {
    public weaponCategory: WeaponCategory
    public name: string;
    public attack: Attack;
    public hitEffect: HitEffect;
    public properties: string[];
}

export enum WeaponCategory {
    Body,
    SimpleMeleeWeapon,
    SimpleRangedWeapon,
    MartialMeleeWeapon,
    MartialRangedWeapon
}