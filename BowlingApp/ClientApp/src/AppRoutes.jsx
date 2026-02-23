import { Home } from "./components/Home";
import Bowling from "./components/Bowling"

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
 
  {
      path: '/bowling',
      element: <Bowling />
  }
];

export default AppRoutes;
