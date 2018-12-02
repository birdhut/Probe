import * as React from "react";
import { ProbeInfo } from "./ProbeApiClient";
import Loader from "./Loader";

import "./probeMenu.css";

interface ProbeMenuProps {
    isLoading: boolean;
    selectedId?: string;
    probes: ProbeInfo[];
    onMenuSelected: (probe: ProbeInfo) => void;
}

const ProbeMenu: React.SFC<ProbeMenuProps> = (props: ProbeMenuProps) => {
    const { isLoading, probes, selectedId, onMenuSelected } = props;
    let content: JSX.Element;
    if (isLoading) {
        content = <Loader />;
    } else {
        const listItems: JSX.Element[] = [];
        for (const p of probes) {
            listItems.push(<li key={p.id} className={selectedId && selectedId === p.id ? "selected" : null}><a href="#" onClick={(event) => 
                {
                    event.preventDefault();
                    onMenuSelected(p);
                }}>{p.id}</a></li>);
        }
        content = <ul>{listItems}</ul>;
    }

    return (
        <nav>
            {content}
        </nav>
    );
};

export default ProbeMenu;
