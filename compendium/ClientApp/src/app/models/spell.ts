import {HitEffect} from "./monster";

export class Spell {
    public id: string;
    public name: string;
    public school: SpellSchool;
    public castAsRitual: boolean;
    public level: number;
    public time: string;
    public range: string;
    public vocalComponent: boolean;
    public somaticComponent: boolean;
    public materials: string;
    public duration: string;
    public text: string;
    public classLists: string[];
    public isMultiTarget: boolean;
    public effects: HitEffect[];
    public castingTime: CastingTime;
    public atHigherLevels: string;
    public atHigherLevelEffects: HitEffect[];
}

export enum SpellSchool
{
    Abjuration = "Abjuration",
    Conjuration = "Conjuration",
    Divination = "Divination",
    Enchantment = "Enchantment",
    Evocation = "Evocation",
    Illusion = "Illusion =",
    Necromancy = "Necromancy",
    Transmutation = "Transmutation"
}

export enum CastingTime
{
    Action = "Action",
    BonusAction = "BonusAction",
    Reaction = "Reaction",
    AttackAction = "AttackAction",
    Minute = "Minute",
    _10Minutes = "_10Minutes",
    Hour = "Hour",
    _8Hours = "_8Hours",
    _12Hours = "_12Hours",
    _24Hours = "_24Hours",
    Varies = "Varies"
}