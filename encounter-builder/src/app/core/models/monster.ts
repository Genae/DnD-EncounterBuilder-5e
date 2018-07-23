export class HitDie {
    public die: number;
    public dieCount: number;
    public offset: number;
    public expectedRoll: number;
    public description: string;
}

export class ChallengeRating {
    public value: number;
    public experience: number;
    public description: string;
}

export class ReadiedSpell {
    public name: string;
    public index: number;
    public marked: boolean;
}

export class Spellcasting {
    public spellcastingLevel: number;
    public spellcastingAbility: string;
    public spellslots: number[];
    public spellDc: number;
    public spellcastingModifier: number;
    public spellListClass: string;
    public spells: ReadiedSpell[][];
    private spellTableStart: number;
    private spellTableEnd: number;
    public textBeforeTable: string;
    public textAfterTable: string;
    public oldTableText: string;
}

export class Monster {
    public name: string;
    public sizeId: number;
    public type: string;
    public alignment: string;
    public armor: string;
    public armorclass: number;
    public maximumHitpoints: number;
    public hitDie: HitDie;
    public speed: string;
    public abilities: { [key: string]: AbilityScore }
    public resist: string;
    public vulnerable: string;
    public immune: string;
    public conditionImmune: string;
    public senses: string;
    public passive: number;
    public languages: string;
    public cr: number;
    public challengeRating: ChallengeRating;
    public size: string;
    public spellcasting: Spellcasting;
    public savingThrows: SavingThrow[];
    public skills: Skill[];
    public traits: Trait[];
    public actions: Action[];
    public reactions: Reaction[];
    public legendaryActions: LegendaryAction[];
}

export class SavingThrow {
    public modifier: number;
    public ability: string;
}

export class AbilityScore {
    public value: number;
    public modifier: number;
}

export class Skill {
    public modifier: number;
    public skill: string;
}

export class Trait {
    public name: string;
    public text: string;
}

export class Action {
    public name: string;
    public text: string;
}

export class Reaction {
    public name: string;
    public text: string;
}

export class LegendaryAction {
    public name: string;
    public text: string;
}
