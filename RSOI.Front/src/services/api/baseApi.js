import jwtDecode from "jwt-decode";
import AuthStore from "../../stores/AuthStore";

export default class BaseApi{
    constructor(axios){
        this.axios = axios;
    }
}