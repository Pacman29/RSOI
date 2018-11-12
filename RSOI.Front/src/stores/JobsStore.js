import {observable, action} from "mobx";


export default class JobsStore {
    @observable isLoading;

    constructor(apiService){
        this.isLoading = false;
        this._apiService = apiService
    }

    @action getAllJobs(){
        this.isLoading = true;
        return this._apiService.API.Jobs.allJobs()
            .then(action((res) => {
            this.isLoading = false;
            return res;
        })).catch(action(e => {
            this.isLoading = false;
            return e;
        })).finally(action(() => {
            this.isLoading = false;
        }));
    }
}