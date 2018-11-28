import { App, View } from 'framework7-react';
import routes from './routes.js';
import React from "react";
import PropTypes from "prop-types";
import {Statusbar} from "framework7-react";
import AuthPage from "./authPage";
import {Block, Button, Page, Panel} from "framework7-react";
import {inject, observer} from "mobx-react";
import AuthStore from "../stores/AuthStore";
import {action} from "mobx";

const f7params = {
    // Array with app routes
    routes,
    // App Name
    name: 'recognize service',
    // App id
    id: 'com.recservice.test',
    theme: 'auto', // Automatic theme detection
};

@inject("authStore")
@observer
export default class Root extends React.Component{
    static propTypes = {
        authStore: PropTypes.instanceOf(AuthStore),
    };

    constructor(props) {
        super(props);
    }

    handleOnViewInit = (evt) => {
    };

    @action handleExit = (evt) => {
        this.props.authStore.exit();
        location.reload();
    };

    render(){
        return (
            <App params={f7params}>
                <AuthPage>
                    <Statusbar/>
                    <Panel right themeDark>
                        <View>
                            <Page>
                                <Block>
                                    <div style={{display: "flex", justifyContent: "center"}}>
                                        <h3>{`Hello, ${this.props.authStore.userName}`}</h3>
                                    </div>
                                    <Button onClick={this.handleExit}>Do you want exit?</Button>
                                </Block>
                            </Page>
                        </View>
                    </Panel>
                    <View id="main-view" main url="/" pushState={true} viewInit={this.handleOnViewInit}/>
                </AuthPage>
            </App>
        )
    }
}