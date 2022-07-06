export class Monster {
    id: string;
    name: string;
    size: Size | string;
    race: MonsterRace;
    alignment: AlignmentDistribution;
    armorInfo: ArmorInfo;
    armor: string;
    armorclass: number;
    maximumHitpoints: number;
    speed: Speed;
    resist: DamageType[] | string[];
    vulnerable: DamageType[] | string[];
    immune: DamageType[] | string[];
    conditionImmune: Condition[] | string[];
    senses: Senses;
    languages: string;
    abilities: { [id: string]: AbilityScore; };
    hitDie: DieRoll;
    challengeRating: ChallengeRating;
    spellcasting: Spellcasting;
    savingThrows: { [id: string]: number; };
    skillmodifiers: { [id: string]: number; };
    traits: Trait[];
    actions: Action[];
    reactions: Reaction[];
    legendaryActions: LegendaryAction[];
    markup: string;
}

export enum ArmorGroup {
    NaturalArmor = "NaturalArmor",
    LightArmor = "LightArmor",
    MediumArmor = "MediumArmor",
    HeavyArmor = "HeavyArmor"
}
export enum ArmorPiece {
    Nat0 = "Nat0",
    Nat1 = "Nat1",
    Nat2 = "Nat2",
    Nat3 = "Nat3",
    Nat4 = "Nat4",
    Nat5 = "Nat5",
    Nat6 = "Nat6",
    Nat7 = "Nat7",
    Nat8 = "Nat8",
    Nat9 = "Nat9",
    Padded = "Padded",
    Leather = "Leather",
    StuddedLeather = "StuddedLeather",
    Hide = "Hide",
    ChainShirt = "ChainShirt",
    ScaleMail = "ScaleMail",
    Brestplate = "Brestplate",
    HalfPlate = "HalfPlate",
    RingMail = "RingMail",
    ChainMail = "ChainMail",
    Splint = "Splint",
    Plate = "Plate"
}

export class ArmorInfo {
    group: ArmorGroup; 
    piece: ArmorPiece | undefined;
    hasShield: boolean;
}

export enum Size {
    Tiny = "Tiny",
    Small = "Small",
    Medium = "Medium",
    Large = "Large",
    Huge = "Huge",
    Gargantuan = "Gargantuan",
    Collosal = "Collosal"
}

export class MonsterRace {
    monsterType: MonsterType | string;
    tags: string;
}

export enum MonsterType {
    Aberration = "Aberration",
    Beast = "Beast",
    Celestial = "Celestial",
    Construct = "Construct",
    Dragon = "Dragon",
    Elemental = "Elemental",
    Fey = "Fey",
    Fiend = "Fiend",
    Giant = "Giant",
    Humanoid = "Humanoid",
    Monstrosity = "Monstrosity",
    Ooze = "Ooze",
    Plant = "Plant",
    Undead = "Undead"
}

export class AlignmentDistribution {
    alignmentChances: AlignmentChance[];
    description: string;
}

export class AlignmentChance {
    alignment: Alignment;
    chance: number;
}

export class Alignment {
    morality: Morality;
    order: Order;
}

export enum Morality {
    Good = "Good",
    Neutral = "Neutral",
    Evil = "Evil"
}
export enum Order {
    Lawful = "Lawful",
    Neutral = "Neutral",
    Chaotic = "Chaotic"
}

export enum Condition {
    Blinded = "Blinded",
    Charmed = "Charmed",
    Deafened = "Deafened",
    Fatigued = "Fatigued",
    Frightened = "Frightened",
    Grappled = "Grappled",
    Incapacitated = "Incapacitated",
    Invisible = "Invisible",
    Paralyzed = "Paralyzed",
    Petrified = "Petrified",
    Poisoned = "Poisoned",
    Prone = "Prone",
    Restrained = "Restrained",
    Stunned = "Stunned",
    Unconscious = "Unconscious",
    Exhaustion = "Exhaustion"
}
export class Speed {
    speeds: { [id: string]: number; };
    additionalInformation: string;
}

export enum MovementType {
    Normal = "Normal",
    Fly = "Fly",
    Hover = "Hover",
    Swim = "Swim",
    Climb = "Climb",
    Burrow = "Burrow"
}

export enum DamageType {
    Acid = "Acid",
    Bludgeoning = "Bludgeoning",
    Cold = "Cold",
    Fire = "Fire",
    Force = "Force",
    Lightning = "Lightning",
    Necrotic = "Necrotic",
    Piercing = "Piercing",
    Poison = "Poison",
    Psychic = "Psychic",
    Radiant = "Radiant",
    Slashing = "Slashing",
    Thunder = "Thunder",
    BludgeoningMagic = "BludgeoningMagic",
    PiercingMagic = "PiercingMagic",
    SlashingMagic = "SlashingMagic"
}

export class Senses {
    passivePerception: number;
    blindOutsideRange: boolean;
    senseRanges: { [id: string]: number; };
    description: string;
}

export enum Sense {
    Blindsight = "Blindsight",
    Darkvision = "Darkvision",
    Tremorsense = "Tremorsense",
    Truesight = "Truesight"
}

export class AbilityScore {
    value: number;
    modifier: number;
    description: string;
}

export class DieRoll {
    die: number;
    dieCount: number;
    offset: number;
    expectedRoll: number;
    description: string;
}

export class ChallengeRating {
    value: number;
    experience: number;
    description: string;
}

export class Spellcasting {
    name: string;
    text: string;
    spellcastingLevel: number;
    spellcastingAbility: Ability;
    spellslots: number[];
    spellDC: number;
    spellcastingModifier: number;
    spellListClass: string;
    spells: PreparedSpell[][];
    textBeforeTable: string;
    textAfterTable: string;
}

export enum Ability {
    Strength = "Strength",
    Dexterity = "Dexterity",
    Constitution = "Constitution",
    Intelligence = "Intelligence",
    Wisdom = "Wisdom",
    Charisma = "Charisma"
}

export class PreparedSpell {
    name: string;
    spellId: string;
    marked: boolean;
}

export class Trait {
    name: string;
    text: string;
}

export class Action {
    name: string;
    text: string;
    attack: Attack;
    hitEffects: HitEffect[];
}

export class HitEffect {
    damageType: DamageType;
    damageDie: DieRoll;
    dC: ICheck;
    condition: Condition[];
}

export interface ICheck {
    value: number;
}

export class AbilityCheck implements ICheck {
    ability: Ability;
    value: number;
}

export class SkillCheck implements ICheck {
    skill: Skill;
    value: number;
}

export class SavingThrow implements ICheck {
    ability: Ability;
    value: number;
}

export class Attack {
    attackBonus: number;
    type: AttackType;
    target: string;
    reach: number;
    shortRange: number;
    longRange: number;
}

export enum AttackType {
    Melee_Weapon_Attack,
    Ranged_Weapon_Attack,
    Melee_or_Ranged_Weapon_Attack,
    Melee_Spell_Attack,
    Ranged_Spell_Attack,
    Melee_or_Ranged_Spell_Attack
}

export enum Skill {
    Acrobatics = 1,
    Animal_Handling = 1 << 1,
    Arcana = 1 << 2,
    Athletics = 1 << 3,
    Deception = 1 << 4,
    History = 1 << 5,
    Insight = 1 << 6,
    Intimidation = 1 << 7,
    Investigation = 1 << 8,
    Medicine = 1 << 9,
    Nature = 1 << 10,
    Perception = 1 << 11,
    Performance = 1 << 12,
    Persuasion = 1 << 13,
    Religion = 1 << 14,
    Sleight_Of_Hand = 1 << 15,
    Stealth = 1 << 16,
    Survival = 1 << 17
}

export class Reaction {
    action: Action;
}
export class LegendaryAction {
    action: Action;
}
