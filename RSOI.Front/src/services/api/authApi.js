import BaseApi from "./baseApi";
import AuthStore from "../../stores/AuthStore";
import jwtDecode from "jwt-decode";

export default class AuthApi extends BaseApi{
    constructor(axios){
        super(axios);
        this._username = undefined;
        this._password = undefined;
        this._tokenUpdater = undefined;
        this.token = this.token;
        this.axios.interceptors.response.use(res => res, error => {
            if(error.response.status === 401){
                let authStore = new AuthStore();
                authStore.isLogin = false;
            }
            return error;
        });
    }

    async login(username,password){
        let res = await this.axios.post("/api/Account",this.loginObject(username,password));

        if(res.status === 200 && res.data) {
            this.token = res.data.token || res.data["Token"];
            this.username = username;
            this.password = password;
            this.setTokenUpdater(this.refreshToken);
            return true;
        }
        else{
            if(res.response.status === 400){
                throw "incorrect login or password";
            }
        }
    }

    async registration(username,password){
        let res = await this.axios.post("/api/Account/Register",this.loginObject(username,password));
        if(res.status === 201 && res.data) {
            this.token = res.data.token || res.data["Token"];
            this.username = username;
            this.password = password;
            this.setTokenUpdater(this.refreshToken);
            return true;
        } else {
            if(res.response.status === 400){
                throw "incorrect data";
            }
        }
    }

    async refreshToken(){
        try {
            console.log("update token");
            let res = await this.axios.get("/api/Account/RefreshToken");
            if(res.status === 200 && res.data) {
                this.token = res.data.token;
                return true;
            }
            return false
        } catch (e) {
            throw e;
        }
    }

    async changePassword(username,oldPassword,password){
        let newuserdata = this.loginObject(username,password);
        newuserdata["OldPassword"] = oldPassword;
        try {
            let res = await this.axios.post("/api/Account/PasswordChange",newuserdata);
            if(res.status === 200 && res.data)
                return true;
        } catch (e) {
            throw e;
        }
    }

    loginObject(username,password){
        return {
            Username : username || this.username,
            Password : password || this._password
        }
    }

    set username(username){
        if(username)
            window.localStorage.setItem("username",username);
    }

    get username(){
        return window.localStorage.getItem("username");
    }

    set password(password){
        if(password)
            this._password = password;
    }

    set token(token){
        window.localStorage.setItem("token",token);
        this.axios.defaults.headers.common['Authorization'] = this.authHeader;
    }

    get token(){
        return window.localStorage.getItem("token")
    }

    get authHeader(){
        console.log(this.token);
        return `Bearer ${this.token}`
    }

    setTokenUpdater(updater){
        clearInterval(this._tokenUpdater);
        let tokenData = jwtDecode(this.token);
        let now = Date.now().valueOf();
        let interval = (tokenData.exp  - (Date.now().valueOf() / 1000))-10;
        console.log(interval);
        this._tokenUpdater = setInterval(updater.bind(this),interval*1000);
    }

    clear(){
        this._username = undefined;
        this._password = undefined;
        this.token = undefined;
        clearInterval(this._tokenUpdater);
    }
}