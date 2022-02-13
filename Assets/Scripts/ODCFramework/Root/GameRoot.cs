/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-02-04 23:55:34 425
 */

public class GameRoot : Root<BaseRoot> {
	public static GameRoot Instance { get; private set; }

	private void Awake() {
		Instance = this;
		Add<ManagerRoot>();
		Add<ModelRoot>();
		// this.Add(ProxyRoot);
	}
}
