
import { configuration } from '@codedoc/core';

import { theme } from './theme';


export const config = /*#__PURE__*/configuration({
  theme,                                  // --> add the theme. modify `./theme.ts` for chaning the theme.
  dest: {
    namespace: '/GitHubIssuePolice'       // --> your github pages namespace. remove if you are using a custom domain.
  },
  page: {
    title: {
      base: 'GitHubIssuePolice'           // --> the base title of your doc pages
    }
  },
  misc: {
    github: {
      user: 'VictorGrycuk',               // --> your github username (where your repo is hosted)
      repo: 'GitHubIssuePolice',          // --> your github repo name
    }
  },
});
