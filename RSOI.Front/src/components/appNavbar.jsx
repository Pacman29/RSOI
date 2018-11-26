import React from "react";
import PropTypes from "prop-types";
import {Icon, NavLeft, NavRight, NavTitle, Navbar, Panel, View, Page, Block, Link, Button} from "framework7-react";
import {inject, observer} from "mobx-react";
import {action} from "mobx";
import AuthStore from "../stores/AuthStore";

@inject("authStore")
@observer
export default class AppNavbar extends React.Component {
    static propTypes = {
        authStore: PropTypes.instanceOf(AuthStore)
    };


    @action handleExit = (evt) => {
        this.props.authStore.exit();
        location.reload();
    };


    render(){
        console.log(` user: ${this.props.authStore.userName}`);
        return (
            <React.Fragment>
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

                <Navbar>
                    <NavLeft backLink="Back"/>
                    <NavTitle>
                        Recognize Service
                    </NavTitle>
                    <NavRight>
                        <Link raised panelOpen="left" iconF7="person"/>
                    </NavRight>
                </Navbar>
            </React.Fragment>
        );
    }
}