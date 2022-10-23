import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { first, map, Observable, startWith } from 'rxjs';
import { Project } from '../../models/project';
import { ProjectService } from '../../services/project.service';


@Component({
    selector: 'projectSelection',
    templateUrl: 'projectSelection.component.html'
})

export class ProjectSelectionComponent {

    separatorKeysCodes: number[] = [ENTER, COMMA];
    projectCtrl = new FormControl('');
    filteredProjects: Observable<Project[]>;
    projects: Project[] = [];
    allProjects: Project[];

    @ViewChild('projectInput') projectInput: ElementRef<HTMLInputElement>;

    constructor(private projectService: ProjectService) {
        this.projectService.getProjects().pipe(first()).subscribe(res => {
            this.allProjects = res;
            this.filteredProjects = this.projectCtrl.valueChanges.pipe(startWith(null), map(proj => this._filter(proj)));
        })
    }

    addProject(event: MatChipInputEvent): void {
        const value = (event.value || '').trim();

        // Add our project
        if (value) {
            let project = this.allProjects.find(p => p.name == value)
            this.projects.push(project!);
        }

        // Clear the input value
        event.chipInput!.clear();

        this.projectCtrl.setValue(null);
    }

    removeProject(project: Project): void {
        const index = this.projects.indexOf(project);

        if (index >= 0) {
            this.projects.splice(index, 1);
        }
        this.projectCtrl.setValue(null);
    }

    selectedProject(event: MatAutocompleteSelectedEvent): void {
        let project = this.allProjects.find(p => p.name == event.option.viewValue)
        this.projects.push(project!);
        this.projectInput.nativeElement.value = '';
        this.projectCtrl.setValue(null);
    }

    private _filter(search: any): Project[] {
        let searchLower = "";
        if (search?.name)
            searchLower = "";
        else
            searchLower = search?.toLowerCase() ?? "";
        return this.allProjects.filter(project => this.projects.indexOf(project) === -1 && project.name.toLowerCase().includes(searchLower));
    }
}
