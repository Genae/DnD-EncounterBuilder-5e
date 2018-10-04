export class Monster {
    name: string;
    size: Size;
    type: MonsterRace;
    alignment: AlignmentDistribution;
    armor: string;
    armorclass: number;
    maximumHitpoints: number;
    speed: Speed;
    resist: DamageType[];
    vulnerable: DamageType[];
    immune: DamageType[];
    conditionImmune: Condition[];
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
}

export enum Size {
    Tiny,
    Small,
    Medium,
    Large,
    Huge,
    Gargantuan,
    Collosal
}

export class MonsterRace {
    type: MonsterType;
    tags: string;
}

export enum MonsterType {
    Aberration,
    Beast,
    Celestial,
    Construct,
    Dragon,
    Elemental,
    Fey,
    Fiend,
    Giant,
    Humanoid,
    Monstrosity,
    Ooze,
    Plant,
    Undead
}

export class AlignmentDistribution {
    alignmentChances: AlignmentChance[];
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
    Good,
    Neutral,
    Evil
}
export enum Order {
    Lawful,
    Neutral,
    Chaotic
}

export enum Condition {
    Blinded,
    Charmed,
    Deafened,
    Fatigued,
    Frightened,
    Grappled,
    Incapacitated,
    Invisible,
    Paralyzed,
    Petrified,
    Poisoned,
    Prone,
    Restrained,
    Stunned,
    Unconscious,
    Exhaustion
}
export class Speed {
    speeds: { [id: string]: number; };
    additionalInformation: string;
}

export enum MovementType {
    Normal,
    Fly,
    Hover,
    Swim,
    Climb,
    Burrow
}

export enum DamageType {
    Acid,
    Bludgeoning,
    Cold,
    Fire,
    Force,
    Lightning,
    Necrotic,
    Piercing,
    Poison,
    Psychic,
    Radiant,
    Slashing,
    Thunder,
    BludgeoningMagic,
    PiercingMagic,
    SlashingMagic
}

export class Senses {
    passivePerception: number;
    blindOutsideRange: boolean;
    senseRanges: { [id: string]: number; };
    description: string;
}

export enum Sense {
    Blindsight,
    Darkvision,
    Tremorsense,
    Truesight
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
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma
}

export class PreparedSpell {
    name: string;
    index: number;
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