import { Component, Input, OnDestroy, ViewChild } from '@angular/core';

import { Router } from '@angular/router';
import { Project } from '../../../models/project';
import { ProjectService } from '../../../services/project.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
    selector: 'projectList',
    templateUrl: 'projectList.component.html'
})

export class ProjectListComponent {

    displayedColumns: string[] = ['name', 'description', 'created', 'updated', 'options'];
    dataSource: MatTableDataSource<Project>;

    projects: Project[] = [];

    @Input() ids: string[];
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    constructor(private projectService: ProjectService, private router: Router) {
        this.projectService.getProjects().subscribe(response => {
            this.projects = response;
            this.dataSource = new MatTableDataSource(this.projects)
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
        });
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.dataSource.filter = filterValue.trim().toLowerCase();

        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    public redirect(id: string) {
        this.router.navigateByUrl('/projectDetail/' + id);
    }

    public edit(project: Project) {
        this.router.navigateByUrl('/projectDetail/' + project.id + "/edit");
    }

    public delete(project: Project) {
        if (confirm("Are you sure to delete " + project.name)) {
            this.projectService.deleteProject(project).subscribe();
        }
    }
}
