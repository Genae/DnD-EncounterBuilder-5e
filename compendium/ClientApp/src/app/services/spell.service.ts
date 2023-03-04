import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Spell } from "../models/spell";


@Injectable()
export class SpellService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    getSpellsWithParams(params: any): Observable<Spell[]> {
        return this.http.get<Spell[]>(this.baseUrl + 'api/spell/filtered', {params});
    }

    getSpells(): Observable<Spell[]> {
        return this.http.get<Spell[]>(this.baseUrl + 'api/spell');
    }

    getSpellById(id: string): Observable<Spell> {
        return this.http.get<Spell>(this.baseUrl + 'api/spell/' + id);
    }

    getSpellsFromIds(ids: string[]): Observable<Spell[]> {
        return this.http.post<Spell[]>(this.baseUrl + 'api/spell/fromIds', ids);
    }
}
