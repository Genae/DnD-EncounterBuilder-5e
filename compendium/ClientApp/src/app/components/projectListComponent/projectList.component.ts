import { Component } from '@angular/core';

import { Router } from '@angular/router';
import { Project } from '../../models/project';
import { ProjectService } from '../../services/project.service';

@Component({
    selector: 'projectList',
    templateUrl: 'projectList.component.html'
})

export class ProjectListComponent {

    projects: Project[] = [];
    search: any;

    constructor(private projectService: ProjectService, private router: Router) {
        this.projectService.getProjects().subscribe(response => this.projects = response);
        this.search = {};
    }

    public redirect(id: string) {
        this.router.navigateByUrl('/projectDetail/' + id);
    }

    public edit(project: Project) {
        this.router.navigateByUrl('/projectDetail/' + project.id + "/edit");
    }

    public delete(project: Project) {
        if (confirm("Are you sure to delete " + name)) {
            this.projectService.deleteProject(project).subscribe();
        }
    }
}
