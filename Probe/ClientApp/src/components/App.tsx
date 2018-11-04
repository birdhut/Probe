import * as React from "react";
import apiClient, { ProbeInfo, ProbeRunArgs } from "./ProbeApiClient";
import { ProbeRunStats, ProbeStats } from "./ProbeStats";
import ProbeConfig from "./ProbeConfig";
import ProbeResult from "./ProbeResult";
import ProbeMenu from "./ProbeMenu";

import "./app.css";

interface AppState {
    loadingMenu: boolean;
    probes: ProbeInfo[];
    selectedProbe?: ProbeInfo;
    loadingResult?: boolean;
    result?: any;
    lastStats?: ProbeRunStats;
}

class App extends React.Component<{}, AppState> {
    constructor(props?: any, context?: any) {
        super(props, context);
        this.state = {
            loadingMenu: true,
            probes: null,
        };
    }

    public componentDidMount(): void {
        apiClient.getProbes()
            .then((probes: ProbeInfo[]) => {
                this.setState({ loadingMenu: false, probes });
            });
    }

    public render(): JSX.Element {

        const { lastStats, loadingMenu, probes, selectedProbe, loadingResult, result } = this.state;

        return (
            <div className="wrapper">
                <div className="header">
                <div className="logo">
                    <h1 className="logo">&#x046A;</h1>
                    <h1>Probe</h1>
                </div>
                <div className="menu">
                    <ProbeMenu isLoading={loadingMenu} selectedId={selectedProbe ? selectedProbe.id : null} probes={probes} onMenuSelected={(probe: ProbeInfo) => this.onProbeSelected(probe)} />
                </div>
                </div>
                <div className="content">
                    <ProbeConfig probe={this.state.selectedProbe} onSendProbe={(args: ProbeRunArgs) => this.onSendProbe(args)} loadingResult={loadingResult} />
                    {lastStats && <ProbeStats {...lastStats} />}
                    {selectedProbe && <ProbeResult id={selectedProbe.id} result={result} isLoading={loadingResult} />}
                </div>
            </div>
        );
    }

    private onProbeSelected(selectedProbe: ProbeInfo): void {
        this.setState({
            selectedProbe,
            lastStats: null,
            loadingResult: null,
            result: null
        });
    }

    private onSendProbe(args: ProbeRunArgs): void {
        this.setState({
            lastStats: null,
            loadingResult: true,
            result: null
        });
        const {selectedProbe} = this.state;
        let lastStats = {
            start: new Date(),
            end: new Date(),
            duration: 0
        };
        apiClient.runProbe(selectedProbe.id, args)
            .then((result: any) => {
                lastStats.end = new Date();
                lastStats.duration = (lastStats.end.valueOf() - lastStats.start.valueOf()) / 1000;
                
                this.setState({
                    lastStats,
                    loadingResult: false,
                    result
                });
            });
    }
}

export default App;
