import {observable, action, computed} from "mobx";

let instance;

export default class AuthStore {
    @observable isLoading;
    @observable isLogin;

    constructor(apiService){
        if(instance)
            return instance;

        this.isLogin = false;
        this.isLoading = false;
        this._apiService = apiService;
        instance = this;
        return this;
    }

    @action login(username,password){
        this.isLoading = true;
        return this._apiService.API.Auth.login(username,password)
            .then(action((res) => {
                this.isLoading = false;
                this.isLogin = res;
                return res
            })).catch(action(e => {
                this.isLoading = false;
                this.isLogin = false;
                throw e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action registration(username,password){
        this.isLoading = true;
        return this._apiService.API.Auth.registration(username,password)
            .then(action((res) => {
                this.isLoading = false;
                this.isLogin = res;
                return res
            })).catch(action(e => {
                this.isLoading = false;
                this.isLogin = false;
                throw e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action refreshToken(){
        this.isLoading = true;
        return this._apiService.API.Auth.refreshToken()
            .then(action((res) => {
                this.isLoading = false;
                this.isLogin = res;
                return res
            })).catch(action(e => {
                this.isLoading = false;
                this.isLogin = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action exit(){
        this._apiService.API.Auth.clear();
    }

    @computed get userName(){
        return this._apiService.API.Auth.username;
    }
}