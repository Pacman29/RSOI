import {computed, observable, action} from "mobx";

export default class DataStore {
    @observable store;
    constructor(){
        this.store = observable({});
        this.store.jobs = observable({});
    }

    createOrUpdateJobs(jobs){
        jobs.forEach(j => this.createOrUpdateJob(j.jobId,j))
    }

    createOrUpdateJob(id,job){
        this.store.jobs[id] = job
    }

    deleteJob(jobId){
        delete this.store.jobs[jobId];
    }

    getJob(jobId){
        return this.store.jobs[jobId];
    }

    getAllJobs(){
        return Object.keys(this.store.jobs).map(key => this.getJob(key));
    }
}