import * as React from "react";
import { ProbeInfo, ProbeRunArgs, ProbeArg } from "./ProbeApiClient";
import ProbeArgConfig from "./ProbeArgConfig";

import "./probeConfig.css";

interface ProbeProps {
    probe?: ProbeInfo;
    onSendProbe: (args: ProbeRunArgs) => void;
}

interface ProbeState {
    args: ProbeRunArgs;
    canSend: boolean;
}

class ProbeConfig extends React.Component<ProbeProps, ProbeState> {
    constructor(props?: ProbeProps, context?: any) {
        super(props, context);
        this.state = {
            args: { },
            canSend: false
        };
    }

    public componentDidMount(): void {
        const canSend: boolean = this.canSend(this.props.probe);
        this.setState({
            canSend
        });
    }

    public componentWillReceiveProps(newProps: ProbeProps): void {
        if ((this.props.probe && newProps.probe && this.props.probe.id !== newProps.probe.id)
                || (!this.props.probe && newProps.probe)
                || (this.props.probe && !newProps.probe)) {
            const canSend: boolean = this.canSend(newProps.probe);
            this.setState({
                args: { },
                canSend
            });
        }
    }

    public render(): JSX.Element {
        const { probe } = this.props;
        const { args, canSend } = this.state;
        const elements: JSX.Element[] = [];

        if (probe) {
            elements.push(<h3 key="probe-header">{probe.id}</h3>);
            elements.push(<p key="probe-description" className="probe-description">{probe.description}</p>);
            elements.push(<ProbeArgConfig key={`probe-arg-config-${probe.id}`} 
                id={probe.id} 
                args={probe.args} 
                onValueSet={(name: string, value: string) => this.onArgValueSet(name, value)} />)

            if (canSend) {
                elements.push(<input key="probe-run" type="button" className="probe-run" onClick={() => this.props.onSendProbe(args)} value="Send Probe" />);
            }

        } else {
            elements.push(<p key="no-probe">Select a probe to run...</p>);
        }
        return (
            <div className="probe-wrap">
                {elements}
            </div>
        );
    }

    private canSend = (probe?: ProbeInfo): boolean => {
        let canSend: boolean = true;
        if (probe) {
            canSend = !probe.args.some((a: ProbeArg) => a.isRequired === true);
        } else {
            canSend = false;
        }
        return canSend;
    }

    private onArgValueSet(name: string, value: string): void {
        const { args } = this.state;
        args[name] = value;

        // test whether we have all required parameters
        const rqdArgs: ProbeArg[] = this.props.probe.args.filter((a: ProbeArg) => a.isRequired === true);
        let canSend: boolean = true;
        for(const a of rqdArgs) {
            if (args[a.name] == null || args[a.name].length < 1) {
                canSend = false;
                break;
            }
        }

        this.setState({
            args,
            canSend
        });
    }
}

export default ProbeConfig;
