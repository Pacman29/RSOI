import { App, View } from 'framework7-react';
import routes from './routes.js';
import React from "react";
import {Statusbar} from "framework7-react";

const f7params = {
    // Array with app routes
    routes,
    // App Name
    name: 'recognize service',
    // App id
    id: 'com.recservice.test',
    theme: 'auto', // Automatic theme detection
};

export default class Root extends React.Component{
    constructor(props) {
        super(props);
    }

    handleOnViewInit = (evt) => {
      debugger;
    };

    render(){
        return (
            <App params={f7params}>
                <Statusbar/>
                <View id="main-view" main url="/" pushState={true} viewInit={this.handleOnViewInit}/>
            </App>
        )
    }
}