import React from "react";
import PropTypes from "prop-types";
import {inject, observer} from "mobx-react";
import AuthStore from "../stores/AuthStore";
import {Page, LoginScreenTitle, List, ListInput, ListButton} from "framework7-react";
import {observable, action, computed} from "mobx";
import Loader from "./loader";

@inject("authStore")
@observer
export default class AuthPage extends React.Component{
    static propTypes = {
      authStore : PropTypes.instanceOf(AuthStore)
    };

    username;
    password;

    constructor(props){
        super(props);
        this.checkAuth()
    }

    @action checkAuth(){
        this.props.authStore.refreshToken();
    }

    @action handlerOnInputUsername = (e) => {
        this.username = e.target.value
    };

    @action handlerOnInputPassword = (e) => {
        this.password = e.target.value
    };

    @action handlerOnSignIn = () => {
        this.props.authStore.login(this.username,this.password).then(action(res => {

        }));
    };

    @action handlerCreateAccount = () => {
        this.props.authStore.registration(this.username,this.password).then(action(res => {

        }));
    };

    render(){
        return this.props.authStore.isLogin ? this.props.children :
            <Page noToolbar noNavbar noSwipeback>
                <Loader isLoading={this.props.authStore.isLoading}>
                    <LoginScreenTitle style={{paddingTop: "10%"}}>Recognize Pdf</LoginScreenTitle>
                    <List inset form>
                        <ListInput
                            label="Username"
                            type="text"
                            placeholder="Your username"
                            value={this.username}
                            onInput={this.handlerOnInputUsername}
                        />
                        <ListInput
                            label="Password"
                            type="password"
                            placeholder="Your password"
                            value={this.password}
                            onInput={this.handlerOnInputPassword}
                        />
                    </List>
                    <List inset >
                        <ListButton style={{textAlign: "center"}} onClick={this.handlerOnSignIn}>Sign In</ListButton>
                        <ListButton style={{textAlign: "center"}} onClick={this.handlerCreateAccount}>Create account</ListButton>
                    </List>
                </Loader>
            </Page>
    }
}