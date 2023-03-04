import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
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
    allProjects: Project[];
    label: string;
    projects: Project[] = [];

    @Input() projectTags: string[]
    @Output() updateList = new EventEmitter<Project[]>();

    @ViewChild('projectInput') projectInput: ElementRef<HTMLInputElement>;

    constructor(private projectService: ProjectService) {
        this.projectService.getProjects().pipe(first()).subscribe(res => {
            this.allProjects = res;
            this.convertTagsToProject();
            this.setLabel();
            this.filteredProjects = this.projectCtrl.valueChanges.pipe(startWith(null), map(proj => this._filter(proj)));
        })
    }

    ngOnChanges(changes: SimpleChanges) {
        this.setLabel();
        this.convertTagsToProject();
    }
    convertTagsToProject() {
        if (this.allProjects)
            this.projects = this.allProjects.filter(p => this.projectTags.indexOf(p.id) !== -1)
    }

    addProject(event: MatChipInputEvent): void {
        const value = (event.value || '').trim();

        // Add our project
        if (value) {
            let project = this.allProjects.find(p => p.name == value)
            this.projects.push(project!);
            this.updateList.emit(this.projects);
        }

        // Clear the input value
        event.chipInput!.clear();

        this.projectCtrl.setValue(null);
        this.setLabel();
    }

    removeProject(project: Project): void {
        const index = this.projects.indexOf(project);

        if (index >= 0) {
            this.projects.splice(index, 1);
            this.updateList.emit(this.projects);
        }
        this.projectCtrl.setValue(null);
        this.setLabel();
    }

    selectedProject(event: MatAutocompleteSelectedEvent): void {
        let project = this.allProjects.find(p => p.name == event.option.viewValue)
        this.projects.push(project!);
        this.updateList.emit(this.projects);
        this.projectInput.nativeElement.value = '';
        this.projectCtrl.setValue(null);
    }

    private setLabel() {
        if (this.projects.length == 0)
            this.label = "Not in any project"
        else
            this.label = "Projects"
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
